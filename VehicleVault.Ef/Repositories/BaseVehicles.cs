global using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;



namespace VehicleVault.Ef.Repositories
{
    public class BaseVehicles : BaseRepository<Vehicle>, IBaseVehicles
    {
        private readonly ApplicationDbContext _context;
        private readonly string _imagePath;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHubContext<VehicleHub> _hubContext;

        public BaseVehicles(ApplicationDbContext context, IUnitOfWork unitOfWork,
            IHttpContextAccessor contextAccessor, IConfiguration configuration, IHubContext<VehicleHub> hubContext) : base(context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _hubContext = hubContext;
            _imagePath = configuration.GetValue<string>("FileSettings:ImagePath")!;
        }

        public async Task<Vehicle> CreateVehicleAsync([FromForm] CreateVehicleDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                  var userEmail = _contextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email).Value;

                Vehicle vehicle = new Vehicle
                {
                    Model = dto.Model,
                    Year = dto.Year,
                    PricePerDay = dto.PricePerDay,
                    PricePerKilometer = dto.PricePerKilometer,
                    DriverSupport = dto.DriverSupport,
                    Street = dto.Street,
                    City = dto.City,
                    StateId = dto.StateId,
                    TypeId = dto.TypeId,
                    Description = dto.Description,
                    CreatedBy = userEmail,
                    CreatedDate = DateTime.UtcNow
                };

                await _context.Vehicles.AddAsync(vehicle);
                  _unitOfWork.Complete();
                // Process images directly from the DTO
                if (dto.Images != null)
                {
                    foreach (var image in dto.Images)
                    {
                        var imagePath = SaveCover(image);
                        _context.Images.Add(new Image { Path = imagePath, VehicleId = vehicle.Id });
                    }
                }



                // Process decorations if provided
                if (dto.DecorationNames != null && dto.DecorationNames.Count > 0)
                {
                    foreach (var dec in dto.GetDecorations())
                    {
                        await _unitOfWork.Decorations.CreateAsync(new Decoration
                        {
                            Name = dec.Name,
                            Price = dec.Price,
                            Description = dec.Description,
                            VehicleId = vehicle.Id
                        });
                    }
                }


                _unitOfWork.Complete();
                await transaction.CommitAsync();
                return vehicle;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while creating the vehicle.", ex);
            }
        }

        public async Task<Vehicle> UpdateVehicleAsync(int vehicleId, UpdateVehicleDto dto)
        {
            string[] includes1 = new string[] { "Images" };
            string[] includes2 = new string[] { "Decorations" };
            var vehicle = await _unitOfWork.BaseVehicles.GetByID(v=>v.Id==vehicleId,includes1,includes2);

            if (vehicle == null)
                throw new KeyNotFoundException("Vehicle not found.");

            // Apply changes to basic properties
            vehicle.Model = dto.Model ?? vehicle.Model;
            vehicle.PricePerDay = dto.PricePerDay ?? vehicle.PricePerDay;
            vehicle.PricePerKilometer = dto.PricePerKilometer ?? vehicle.PricePerKilometer;
            vehicle.DriverSupport = dto.DriverSupport ?? vehicle.DriverSupport;
            vehicle.Street = dto.Street ?? vehicle.Street;
            vehicle.City = dto.City ?? vehicle.City;
            vehicle.Description = dto.Description ?? vehicle.Description;
            vehicle.Year = dto.Year ?? vehicle.Year;
            vehicle.StateId = dto.StateId ?? vehicle.StateId;
            vehicle.TypeId = dto.TypeId ?? vehicle.TypeId;
            vehicle.UpdatedDate = DateTime.UtcNow;

            // Handle image updates: deletion and addition of new images
            if (dto.DeleteImageIds != null && dto.DeleteImageIds.Count > 0)
            {
                var imagesToDelete = vehicle.Images.Where(img => dto.DeleteImageIds.Contains(img.Id)).ToList();
                _context.Images.RemoveRange(imagesToDelete);
            }

            if (vehicle.Images == null)
            {
                vehicle.Images = new List<Image>();
            }
            foreach (var newImage in dto.NewImages ?? new List<IFormFile>())
            {
                var imagePath = SaveCover(newImage);  // Assuming SaveCover returns the path
                vehicle.Images.Add(new Image { Path = imagePath ,VehicleId=vehicleId});
            }

            // Handle decoration updates: add, update, and delete logic

            var decorations = dto.GetDecorations();
            foreach (var decorationDto in decorations)
            {
                if (decorationDto.IsDeleted)
                {
                    var decoration = vehicle.Decorations.FirstOrDefault(d => d.Id == decorationDto.Id);
                    if (decoration != null)
                    {
                        _context.Decorations.Remove(decoration);
                    }
                }
                else if (decorationDto.Id.HasValue && decorationDto.Id > 0)
                {
                    // Update existing decoration
                    var decoration = vehicle.Decorations.FirstOrDefault(d => d.Id == decorationDto.Id);
                    if (decoration != null)
                    {
                        decoration.Name = decorationDto.Name;
                        decoration.Price = decorationDto.Price;
                        decoration.Description = decorationDto.Description;
                    }
                }
                else
                {
                    // Add new decoration
                    vehicle.Decorations.Add(new Decoration
                    {
                        Name = decorationDto.Name,
                        Price = decorationDto.Price,
                        Description = decorationDto.Description
                    });
                }
            }

            _context.Vehicles.Update(vehicle);
             _unitOfWork.Complete();

            return vehicle;
        }

        public async Task<VehicleDetailsDto> Details(int id)
        {

            string[] includes1 = new string[] { "Images" };
            string[] includes2 = new string[] { "Decorations" };
            string[] includes3 = new string[] { "Type" };
            var vehicle = await _unitOfWork.BaseVehicles.GetByID(v => v.Id == id, includes1, includes2, includes3);

            if (vehicle == null)
                throw new KeyNotFoundException("Vehicle not found.");

            VehicleDetailsDto vehicleMap = new()
            {
                City = vehicle.City,
                CreatedBy = vehicle.CreatedBy,
                CreatedDate = vehicle.CreatedDate,
                DriverSupport = vehicle.DriverSupport,
                Model = vehicle.Model,
                Street = vehicle.Street,
                PricePerDay = vehicle.PricePerDay,
                PricePerKilometer = vehicle.PricePerKilometer,
                VehicleType = vehicle.Type.Name,
                Year = vehicle.Year,
                Description = vehicle.Description,
                Images = vehicle.Images.Select(i => i.Path).ToList(),
                Decorations = vehicle.Decorations.Select(d => new DecorationDto
                {
                    Name = d.Name,
                    Price = d.Price,
                    Description = d.Description
                }).ToList()
            };
            return vehicleMap;
        }

        public async Task<IEnumerable<VehicleDetailsDto>> ReadAll(string useremail=null)
        {
            string[] includes1 = new string[] { "Images" };
            string[] includes2 = new string[] { "Decorations" };
            string[] includes3 = new string[] { "Type" };
            var vehicles = await _unitOfWork.BaseVehicles.ReadAsync(c=>c.CreatedBy==useremail||useremail==null,includes1, includes2, includes3);
           
            var vehicle=vehicles.Select(v=> new VehicleDetailsDto
            {
                City = v.City,
                CreatedBy = v.CreatedBy,
                CreatedDate = v.CreatedDate,
                DriverSupport = v.DriverSupport,
                Model = v.Model,
                Street = v.Street,
                PricePerDay = v.PricePerDay,
                PricePerKilometer = v.PricePerKilometer,
                VehicleType = v.Type.Name,
                Year = v.Year,
                Description = v.Description,
                Images = v.Images.Select(i => i.Path).ToList(),
                Decorations = v.Decorations.Select(d => new DecorationDto
                {
                    Name = d.Name,
                    Price = d.Price,
                    Description = d.Description
                }).ToList()
            }).ToList();
            return vehicle;
        }

        private string SaveCover(IFormFile cover)
        {
            if (cover is null)
                return null;

            var coverName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}";

            var path = Path.Combine(_imagePath, coverName);


            using var stream = File.Create(path);
            cover.CopyTo(stream);

            return coverName;
        }

        public async Task UpdateVehicleAvailability()
        {
            var vehicles = await _unitOfWork.BaseVehicles.ReadAsync();
            foreach (var vehicle in vehicles)
            {
                var isCurrentlyRented = _context.Rentals.Any(r => r.VehicleId == vehicle.Id && r.IsActive && r.StarDate <= DateTime.Now && r.EndDate >= DateTime.Now);
                bool oldAvailability = vehicle.IsAvailable;
                vehicle.IsAvailable = !isCurrentlyRented;

                if (vehicle.IsAvailable != oldAvailability) // Only update and notify if there's a change
                {
                    _unitOfWork.BaseVehicles.UpdateAsync(vehicle);
                    await _hubContext.Clients.All.SendAsync("UpdateVehicleAvailability", vehicle.Id, vehicle.IsAvailable);
                }
            }
            _unitOfWork.Complete();
        }
    }
}

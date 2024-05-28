using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace VehicleVault.Ef.Repositories
{
    public class BaseRental : BaseRepository<Rental>, IBaseRental
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly string _imagePath;
        private readonly ApplicationDbContext _context;

        public BaseRental(IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor, ApplicationDbContext context, IConfiguration configuration) : base(context)
        {
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
            _context = context;
            _imagePath = configuration.GetValue<string>("FileSettings:ImagePath")!;

        }


        public async Task<RentalDto> CreateRental(RentalDto rentalDto)
        {
            var useremail = _contextAccessor.HttpContext.User.Claims.First(u => u.Type.Equals(ClaimTypes.Email)).Value;


            string[] includes1 = new string[] { "Decorations" };
            string[] includes2 = new string[] { "Type" };
            var vehicle = await _unitOfWork.BaseVehicles.GetByID(i => i.Id == rentalDto.VehicleId, includes1, includes2);
            if (vehicle == null)
                throw new Exception("Vehicle not found");


            decimal totalCost = 0;
            int offerId = 0;
            // Check the type of vehicle and whether a driver is included
            if (vehicle.Type.Name.Equals("Car", StringComparison.OrdinalIgnoreCase))
            {

                if (rentalDto.DriverIncluded)
                {
                    // Car with driver, pay per kilometer
                    totalCost = CalculateCostPerKilometer(vehicle, rentalDto);
                }
                else
                {

                    // Car without driver, pay per day
                    totalCost = CalculateCostPerDay(vehicle, rentalDto);
                    totalCost += await CalculateDecorationCosts(rentalDto.Decorations);

                    
                    if (!rentalDto.PromoCode.IsNullOrEmpty())
                    {
                        var offer = await _unitOfWork.Offers.GetByID(o => o.Promocode == rentalDto.PromoCode);
                        if (offer != null)
                        offerId += offer.Id;

                        var IsValid = await _unitOfWork.OffersServices.ValidatePromoCodeAsync(rentalDto.PromoCode);
                        if (IsValid)
                        {
                            var discountAmount = await _unitOfWork.OffersServices.GetDiscountAmountAsync(rentalDto.PromoCode);
                            var totalDiscount = totalCost * (discountAmount / 100);
                            totalCost -= totalDiscount;
                        }
                        offer.IsUsed = true;
                        
                        _unitOfWork.Offers.UpdateAsync(offer);
                        _unitOfWork.Complete();
                    }
                }

            }
            else if (vehicle.Type.Name.Equals("Truck", StringComparison.OrdinalIgnoreCase))
            {
                // Trucks are always with driver and pay per kilometer
                totalCost = CalculateCostPerKilometer(vehicle, rentalDto);
            }

            if (await IsVehicleAvailableForRental(vehicle.Id, rentalDto.StartDate, rentalDto.EndDate))
            {


                var rental = new Rental
                {
                    CreatedBy = useremail,
                    CreatedDate = DateTime.UtcNow,
                    VehicleId = rentalDto.VehicleId,
                    StarDate = rentalDto.StartDate,
                    EndDate = rentalDto.EndDate,
                    DriverIncluded = rentalDto.DriverIncluded,
                    TotalCost = totalCost,
                    OfferId=offerId,
                    RentalDecorations = AddDecorations(rentalDto.Decorations),
                    FrontCard=SaveCover(rentalDto.FrontCardImg),
                    BackCard=SaveCover(rentalDto.BackCardImg)
                };
                await _unitOfWork.BaseRentals.CreateAsync(rental);
                _unitOfWork.Complete();
                return new RentalDto { Massege="Rented"};
            }
            else
            {
                return new RentalDto { Massege="Already Rented"} ;
            }

        }

        private decimal CalculateCostPerDay(Vehicle vehicle, RentalDto rentalDto)
        {
            var days = (int)(rentalDto.EndDate - rentalDto.StartDate).TotalDays;
            var pricePerDay = vehicle.PricePerDay ?? 0m; // Default to 0 if price per day is null
            return days * pricePerDay;
        }


        private decimal CalculateCostPerKilometer(Vehicle vehicle, RentalDto rentalDto)
        {
            // This assumes that the calculation of the actual distance and total cost will be done manually
            return vehicle.PricePerKilometer ?? 0; // Returns just the cost per kilometer as a reference
        }

        private async Task<decimal> CalculateDecorationCosts(int[] decorationIds)
        {
            var decorations = await _unitOfWork.Decorations.ReadAsync(d => decorationIds.Contains(d.Id));
            return decorations.Sum(d => d.Price);
        }

        private List<RentalDecoration> AddDecorations(int[] decorationIds)
        {
            return decorationIds.Select(id => new RentalDecoration { DecorationId = id }).ToList();
        }

        private async Task<bool> IsVehicleAvailableForRental(int vehicleId, DateTime startDate, DateTime endDate)
        {
            var rentalsForTheVehicle = await _unitOfWork.BaseRentals.ReadAsync(r => r.VehicleId == vehicleId);
            return !rentalsForTheVehicle.Any(rental =>
                startDate.Date <= rental.EndDate.Date && rental.StarDate.Date <= endDate.Date);
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


        public async Task<bool> CanUserAddComment(string useremail, int vehicleId)
        {
            bool hasRented = await _context.Rentals.AnyAsync(u => u.CreatedBy == useremail && u.VehicleId == vehicleId);
            return hasRented;
        }


        public async Task<IEnumerable<DisplayRentsForAdminVehciles>> GetRentsForAdmin()
        {
            var userEmail = _contextAccessor.HttpContext.User.Claims.First(u => u.Type.Equals(ClaimTypes.Email)).Value;
            if (userEmail == null)
                return null;

          
            var rents = await _unitOfWork.BaseRentals.ReadAsync(filter: r => r.Vehicle.CreatedBy == userEmail);

            var rentalData = rents.Select(r => new DisplayRentsForAdminVehciles
            {
                CreatedBy = r.CreatedBy,
                CreatedDate = r.CreatedDate,               
                Model = r.Vehicle.Model,               
                TotalCost = r.TotalCost,
                StartDate = r.StarDate,
                EndDate = r.EndDate,
            });


            return rentalData;
        }

        public async Task<IEnumerable<DisplayRentsForUser>> GetRentsForUser()
        {
            var userEmail = _contextAccessor.HttpContext.User.Claims.First(u => u.Type.Equals(ClaimTypes.Email)).Value;
            if (userEmail == null)
                return null;

            var rents = await _unitOfWork.BaseRentals.ReadAsync(filter: r => r.CreatedBy == userEmail);

            var rentalData = rents.Select(r => new DisplayRentsForUser
            {
                Model = r.Vehicle.Model,
                TotalCost = r.TotalCost,
                StartDate = r.StarDate,
                EndDate = r.EndDate,
            });


            return rentalData;
        }

        public async Task<IEnumerable<DisplayRentsForManger>> GetRentsForManger()
        {
            var userEmail = _contextAccessor.HttpContext.User.Claims.First(u => u.Type.Equals(ClaimTypes.Email)).Value;
            if (userEmail == null)
                return null;
            string[] includes1 = new string[] { "Vehicle" };
            var rents = await _unitOfWork.BaseRentals.ReadAsync(null,includes1);

            var rentalData = rents.Select(r => new DisplayRentsForManger
            {
                Model = r.Vehicle.Model,
                TotalCost = r.TotalCost,
                StartDate = r.StarDate,
                EndDate = r.EndDate,
                CreatedBy=r.Vehicle.CreatedBy,
                CreatedDate=r.Vehicle.CreatedDate,
                DriverIncluded=r.DriverIncluded,
                RentedBy=r.CreatedBy
            });
            return rentalData;
        }
    }

}


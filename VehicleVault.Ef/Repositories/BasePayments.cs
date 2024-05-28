using Microsoft.AspNetCore.Hosting;

namespace VehicleVault.Ef.Repositories
{
    public class BasePayments : BaseRepository<Payment>, IBasePayments
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BasePayments(ApplicationDbContext context, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment) : base(context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }



        public async Task ProcessPayment(int rentalId, byte methodId)
        {

            var rental = await _unitOfWork.BaseRentals.GetByID(i => i.Id == rentalId);
            if (rental == null)
                throw new InvalidOperationException("Rental not found.");

            var paymentMethod = await _unitOfWork.PaymentMethods.GetByID(i => i.Id == methodId);
            if (paymentMethod == null)
                throw new InvalidOperationException("Payment method not found.");

            if (paymentMethod.Name.Equals("Cash", StringComparison.OrdinalIgnoreCase))
            {
                await ProcessCashPayment(rental, paymentMethod);
            }
            else if (paymentMethod.Name.Equals("Credit", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException("Credit card payment is not available at this time.");
            }
        }

        private async Task ProcessCashPayment(Rental rental, PaymentMethod paymentMethod)
        {
            if (rental.Payment == null) // Check if payment hasn't been made already
            {
                rental.Payment = new Payment
                {
                    RentalId = rental.Id,
                    MethodId = paymentMethod.Id,
                    Amount = rental.TotalCost,
                    PaymentDate = DateTime.Now,
                    PaymentMethod = paymentMethod,
                };

                rental.IsPaid = true;
                _unitOfWork.BaseRentals.UpdateAsync(rental);
                await _unitOfWork.BasePayments.CreateAsync(rental.Payment);
                _unitOfWork.Complete();

                await SendMail(rental.Id);

            }
            else
            {
                throw new InvalidOperationException("Payment has already been processed for this rental.");
            }
        }
        private async Task SendMail(int rentId)
        {
            string[] includes = new string[] { "RentalDecorations" };
            var rental = await _unitOfWork.BaseRentals.GetByID(i => i.Id == rentId, includes);
            var decIds = rental.RentalDecorations.Select(i => i.DecorationId).ToList();

            string[] includes1 = new string[] { "Images" };
            string[] includes2 = new string[] { "Decorations" };
            var vehicle = await _unitOfWork.BaseVehicles.GetByID(i => i.Id == rental.VehicleId, includes1, includes2);

            var vehicleDecorations = vehicle.Decorations.Where(d => decIds.Contains(d.Id)).ToList();

            // Check if vehicleDecorations is empty and proceed
            if (vehicleDecorations.Count == 0)
            {
                throw new InvalidOperationException();
                // Optionally, you can add a log here or handle this case specifically
            }

            var admin = await _userManager.FindByEmailAsync(vehicle.CreatedBy);
            var filePath = Path.Combine(
               Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
               "Project Vehicle Rentals",
               "VehicleVault",
               "VehicleVault.Api",
               "Templates",
               "Rental.html");
            var str = new StreamReader(filePath);

            var mailText = await str.ReadToEndAsync();
            str.Close();
            mailText = mailText.Replace("[user]", admin.FullName)
                .Replace("[Model]", vehicle.Model)
                .Replace("[WithDriver]", rental.DriverIncluded.ToString())
                .Replace("[TotalCost]", rental.TotalCost.ToString())
                .Replace("[StartDate]", rental.StarDate.Date.ToString("yyyy-MM-dd"))
                .Replace("[EndDate]", rental.EndDate.Date.ToString("yyyy-MM-dd"))
                .Replace("[ImageUrl]", GetImageUrl(vehicle.Images.Take(1).Select(p => p.Path).FirstOrDefault()));

            // Only include decorations in the email if there are any
            if (vehicleDecorations.Count > 0)
            {
                mailText = mailText.Replace("[Decorations]", string.Join(", ", vehicleDecorations.Select(n => n.Name)));
            }
            else
            {
                mailText = mailText.Replace("[Decorations]", "No decorations");
            }

            await _unitOfWork.MailServices.SendEmailAsync(vehicle.CreatedBy, "Vehicle Rental Details", mailText);
        }






        private string GetImageUrl(string imageName)
        {
            // Base URL of the hosted application. This should be replaced with the actual domain where your application is hosted.
            string baseUrl = "https://your-domain.com/";

            // Construct the relative URL for the image
            string imageUrl = Path.Combine(baseUrl, "Images", imageName);

            return imageUrl;
        }

    }

}
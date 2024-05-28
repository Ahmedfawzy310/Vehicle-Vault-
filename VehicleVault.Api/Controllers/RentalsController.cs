namespace VehicleVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public RentalsController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        [HttpPost("rent")]
        [Authorize]
        public async Task<IActionResult> CreateRental([FromForm] RentalDto rentalDto)
        {
            var userId = User.FindFirst(ClaimTypes.PrimarySid)?.Value;
            if (userId is null)
                return Unauthorized("UnAuhtorized");

            await _unitOfWork.BaseRentals.CreateRental(rentalDto);
            var x = rentalDto.Massege;
            return Ok();

        }

        [HttpPost("pay")]
        [Authorize]
        public async Task<IActionResult> ProcessPayment(int rentalId, byte methodId)
        {
            try
            {
                await _unitOfWork.BasePayments.ProcessPayment(rentalId, methodId);
                return Ok("Payment processed successfully.");
            }
            catch (NotSupportedException e)
            {
                return BadRequest(e.Message);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}

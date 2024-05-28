using VehicleVault.Core.DTOS.Auth;
using VehicleVault.Core.DTOS.User;
using VehicleVault.Core.Entities;

namespace VehicleVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public AuthController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        /*=====================================================================================================================*/

        /* Register & Confirm Email */

        /*=====================================================================================================================*/


        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromForm] RegisterModel model)
        {
          
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var result = await _unitOfWork.UserServices.RegisterAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);


            return Ok(result);
        }

        [HttpPost("emailConfirm")]
        public async Task<IActionResult> RegisterConfirm(string userId, string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
                return BadRequest("Invalid user ID or code.");

            await _unitOfWork.UserServices.RegisteConfirm(userId, code);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
               return NotFound("User not found.");
            

            var promoCodeDto = new PromoCodeDto
            {
                DiscountAmount = 10,
                ExpirationDate = DateTime.Now.AddDays(30)
            };
            var promoCode = await _unitOfWork.OffersServices.GeneratePromoCodeAsync(promoCodeDto);

            var welcomeDto = new WelcomeDto
            {
                Email = user.Email,
                Name = user.FullName,
                PromoCode = promoCode.Promocode
            };
            var emailContent = await LoadEmailTemplate(welcomeDto, promoCode);

    
            await _unitOfWork.MailServices.SendEmailAsync(welcomeDto.Email, "Welcome to our Website", emailContent, null);

            return Ok("Email Confirmed");
        }

        private async Task<string> LoadEmailTemplate(WelcomeDto welcome, Offer promoCode)
        {
            var filePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "Project Vehicle Rentals",
                "VehicleVault",
                "VehicleVault.Api",
                "Templates",
                "Welcome.html");

            using var reader = new StreamReader(filePath);
            var mailText = await reader.ReadToEndAsync();

            mailText = mailText.Replace("[Username]", welcome.Name)
                               .Replace("[promoCode]", welcome.PromoCode)
                               .Replace("[expire]", promoCode.ExpireDate.ToString())
                               .Replace("[discount]", ((double)promoCode.DiscountPercent).ToString());

            return mailText;
        }


        /*=====================================================================================================================*/

        /* Login & LogOut */

        /*=====================================================================================================================*/

        [HttpPost("login")]
        public async Task<IActionResult> GetTokenAsync([FromForm] TokenRequestModel model)
        {
            var result = await _unitOfWork.UserServices.GetTokenAsync(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenModel model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");

            var result = await _unitOfWork.UserServices.RevokeTokenAsync(token);

            if (!result)
                return BadRequest("Token is invalid!");

            return Ok();
        }
        /*=====================================================================================================================*/

        /* Roles & Refresh Token */

        /*=====================================================================================================================*/
       

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            var result = await _unitOfWork.UserServices.RefreshTokenAsync(refreshToken);

            if (!result.IsAuthenticated)
                return BadRequest(result);

            SetRefreshTokenInCookie(result.RefreshToken!, result.RefreshTokenExpiration);

            return Ok(result);
        }
        /*=====================================================================================================================*/

        /* Forget Password */

        /*=====================================================================================================================*/
        [HttpPost("resetPassword")]
        public async Task<IActionResult> SendResetCode([FromBody] RessetPassword model)
        {
            var email = model.Email;

            var result = await _unitOfWork.UserServices.SendResetPasswordCodeAsync(email);

            if (result)
            {
                return Ok(new { Message = "Reset code sent successfully. Please check your email for instructions." });
            }

            return BadRequest(new { Message = "User not found." });
        }

        [HttpPost("newPassword")]
        public async Task<IActionResult> CompleteResetPassword([FromBody] ConfirmResetPassword model)
        {
            var email = model.Email;
            var code = model.Code;
            var newPassword = model.NewPassword;

            var success = await _unitOfWork.UserServices.ResetPasswordAsync(email, code, newPassword);

            if (success)
            {
                return Ok(new { Message = "Password reset successfully." });
            }

            return BadRequest(new { Message = "Invalid user or reset code." });
        }
        /*=====================================================================================================================*/




        private void SetRefreshTokenInCookie(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime(),
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}

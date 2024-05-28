global using VehicleVault.Core.Interfaces;
global using VehicleVault.Ef.Data;
global using System.Security.Claims;
global using VehicleVault.Core.DTOS;
global using Microsoft.AspNetCore.Authorization;
using VehicleVault.Core.DTOS.Auth;
using VehicleVault.Core.DTOS.User;

namespace VehicleVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ModifyProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public ModifyProfileController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
               return Unauthorized("User not authenticated.");
            

            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user is null)
                return NotFound("User not found.");

            var state = await _unitOfWork.States.GetByID(g => g.Id == user.StateId);
            if (state is null)
                return NotFound("Governorate not found.");

            var userProfile = new ProfileDto
            {
                FullName = user.FullName,
                StateName = state.Name,
                Email = user.Email,
                Phone = user.PhoneNumber,
                StateId = user.StateId
            };

            return Ok(userProfile);
        }

        [HttpPut("updateProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] ProfileDto model)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("User not authenticated.");

            var user = await _userManager.FindByIdAsync(userEmail);
            if (user is null)
                return NotFound("User not found.");

            if (!string.IsNullOrWhiteSpace(model.FullName))
                user.FullName = model.FullName;

            if (model.StateId.HasValue)
                user.StateId = model.StateId.Value;

            if (!string.IsNullOrWhiteSpace(model.Phone))
                user.PhoneNumber = model.Phone;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok("Profile updated successfully.");

            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(string.Join("; ", errors));
        }

        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            if (model is null)
                return BadRequest("Invalid data provided.");

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null)
                return NotFound("User not found.");

            var passwordCheckResult = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
            if (!passwordCheckResult)
                return BadRequest("Incorrect current password.");


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (result.Succeeded)
                return Ok("Password changed successfully.");

            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(string.Join("; ", errors));
        }

        [HttpGet("myRentals")]
        public async Task<IActionResult> UserRentals()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("User not authenticated.");

            var rentals = await _unitOfWork.BaseRentals.GetRentsForUser();

            return Ok(rentals);
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("adminRenals")]
        public async Task<IActionResult> AdminRentals()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("User not authenticated.");

            var rentals = await _unitOfWork.BaseRentals.GetRentsForAdmin();

            return Ok(rentals);
        }

    }
}

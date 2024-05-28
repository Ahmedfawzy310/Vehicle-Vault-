using Microsoft.EntityFrameworkCore;
using VehicleVault.Core.DTOS.User;

namespace VehicleVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;

        public UsersController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IHttpContextAccessor contextAccessor)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _contextAccessor = contextAccessor;
        }

        [Authorize(Roles = "Manger")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.FullName) 
                .Select(user => new UserDto
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult(),
                })
                .ToListAsync();

            return Ok(users);
        }

        [Authorize(Roles = "Manger")]
        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.Select(n => n.Name).ToListAsync();
            return Ok(roles);
        }

        [HttpGet("manageRoles/{userid}")]
        public async Task<IActionResult> ManageRoles(string userid)
        {
            var user = await _userManager.FindByIdAsync(userid);

            if (user is null)
                return NotFound();

            var roles = await _roleManager.Roles.ToListAsync();

            UserRolesDto dto = new()
            {
                UserId = user.Id,
                UserName = user.UserName!,
                Roles = roles.Select(role => new RolesDto
                {
                    RoleName = role.Name!,
                    IsSelected = _userManager.IsInRoleAsync(user, role.Name).GetAwaiter().GetResult()
                }).ToList(),
            };
            return Ok(dto);
        }

        [Authorize(Roles = "Manger")]
        [HttpPost("userRoles")]
        public async Task<IActionResult> ManageRoles(UserRolesDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);

            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in dto.Roles)
            {
                if (userRoles.Any(r => r == role.RoleName) && !role.IsSelected)
                    await _userManager.RemoveFromRoleAsync(user, role.RoleName);

                if (!userRoles.Any(r => r == role.RoleName) && role.IsSelected)
                    await _userManager.AddToRoleAsync(user, role.RoleName);
            }

            return Ok(dto);
        }

        [Authorize(Roles = "Manger")]
        [HttpPost("newRole")]
        public async Task<IActionResult> Add(AddRoleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(await _roleManager.Roles.ToListAsync());


            if (await _roleManager.RoleExistsAsync(dto.Name))
            {
                ModelState.AddModelError("Name", "Role is Exits");
                return BadRequest(await _roleManager.Roles.ToListAsync());
            }

            await _roleManager.CreateAsync(new IdentityRole(dto.Name.Trim()));
            return Ok(dto);
        }

        [HttpPost("requestAdmin")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RequestAdminRole([FromForm] RequestToBeAdminDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.PrimarySid)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null)
                return Unauthorized("Go to Register First");

            dto.UserId = userId;

            var result = await _unitOfWork.AdminRequestsServices.RequestAdminRole(dto);
            if (result is null)
                return BadRequest("Request not accepted");

            var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Vehicel Rental", "VehicleVault", "VehicleVault.Api", "Templates", "Request.html");
            var mailText = await System.IO.File.ReadAllTextAsync(filePath);
            mailText = mailText.Replace("[Username]", user.FullName);

            await _unitOfWork.MailServices.SendEmailAsync(user.Email, "Admin Role", mailText, null, null);

            return Ok();
        }

        [HttpGet("pendingRequests")]
        [Authorize(Roles = "Manger")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var pendingRequests = await _unitOfWork.AdminRequestsServices.GetPendingRequestsAsync();
            return Ok(pendingRequests);
        }

        [HttpPost("approveRequest/{requestId}")]
        [Authorize(Roles = "Manger")]
        public async Task<IActionResult> ApproveRequest(int requestId)
        {
            var request = await _unitOfWork.AdminRequests.GetByID(r => r.Id == requestId);
            if (request is null)
                return NotFound("Request not found.");

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
                return NotFound("User not found.");

            var success = await _unitOfWork.AdminRequestsServices.ApproveRequestAsync(requestId);
            if (success)
            {
                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Graduation Project", "VehicleVault", "VehicleVault.Api", "Templates", "approved.html");
                var mailText = await System.IO.File.ReadAllTextAsync(filePath);
                mailText = mailText.Replace("[Username]", user.FullName);

                await _unitOfWork.MailServices.SendEmailAsync(user.Email!, "Admin Role", mailText, null, null);
                return Ok("Request approved successfully.");
            }
            else
            {
                return NotFound("Unable to approve request.");
            }
        }

        [HttpPost("rejectRequest/{requestId}")]
        [Authorize(Roles = "Manger")]
        public async Task<IActionResult> RejectRequest(int requestId)
        {
            var request = await _unitOfWork.AdminRequests.GetByID(r => r.Id == requestId);
            if (request == null)
                return NotFound("Request not found.");

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound("User not found.");

            var success = await _unitOfWork.AdminRequestsServices.RejectRequestAsync(requestId);
            if (success)
            {
                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Vehicel Rental", "VehicleVault", "VehicleVault.Api", "Templates", "reject.html");
                var mailText = await System.IO.File.ReadAllTextAsync(filePath);
                mailText = mailText.Replace("[Username]", user.FullName);

                await _unitOfWork.MailServices.SendEmailAsync(user.Email!, "Admin Role", mailText, null, null);
                return Ok("Request rejected successfully.");
            }
            else
            {
                return NotFound("Request not found or unable to reject.");
            }
        }



        [Authorize(Roles = "Manger")]
        [HttpGet("renals")]
        public async Task<IActionResult> Rentals()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
                return Unauthorized("User not authenticated.");

            var rentals = await _unitOfWork.BaseRentals.GetRentsForManger();

            return Ok(rentals);
        }

    }
}

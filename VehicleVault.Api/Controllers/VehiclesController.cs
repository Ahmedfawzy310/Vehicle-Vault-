global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Mvc;


namespace VehicleVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public VehiclesController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles ="Admin")]
        [HttpPost("newVehicle")]
        public async Task<IActionResult> CreateAsync([FromForm] CreateVehicleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var vehicle = await _unitOfWork.BaseVehicles.CreateVehicleAsync(dto);
                return Ok("vehicle saved");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPatch("update/{vehicleId}")]
        public async Task<IActionResult> UpdateVehicle(int vehicleId, [FromForm] UpdateVehicleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var vehicle=await _unitOfWork.BaseVehicles.GetByID(i=>i.Id== vehicleId);
            var useremail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (useremail == null)
                return Unauthorized("User not authenticated");

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole != "Manger" || useremail != vehicle.CreatedBy)
                return BadRequest("Not your buisness");

            try
            {
                var updatedVehicle = await _unitOfWork.BaseVehicles.UpdateVehicleAsync(vehicleId, dto);
                return Ok("updatedVehicle");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vehicle = await _unitOfWork.BaseVehicles.GetByID(v => v.Id == id);
            if (vehicle == null)
                return NotFound();

            var useremail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (useremail == null)
                return Unauthorized("User not authenticated");

            if (useremail != vehicle.CreatedBy || userRole != "Manger")
                return BadRequest("it is not your car");

            _unitOfWork.BaseVehicles.DeleteAsync(vehicle);
            _unitOfWork.Complete();

            return Ok("deleted");
        }


        [HttpGet("details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var vehicle = await _unitOfWork.BaseVehicles.Details(id);
            return Ok(vehicle);
        }

        [HttpGet("vehicles")]
        public async Task<IActionResult> ReadAll()
        {
            return Ok(await _unitOfWork.BaseVehicles.ReadAll());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("ownedVehicles")]
        public async Task<IActionResult> GetMyVehciles()
        {
            var useremail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (useremail == null)
                return NotFound("No User");

            var vehciles = await _unitOfWork.BaseVehicles.ReadAll(useremail);
            return Ok(vehciles);
        }

        [HttpPost("updateAvailability")]
        public async Task<IActionResult> UpdateVehicleAvailability()
        {
            await _unitOfWork.BaseVehicles.UpdateVehicleAvailability();
            return Ok("Vehicle availability updated.");
        }

    }
}

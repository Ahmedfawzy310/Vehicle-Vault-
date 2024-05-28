using VehicleVault.Core.Entities;

namespace VehicleVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TypesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public TypesController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        [Authorize(Roles = "Manger")]
        [HttpPost("newType")]
        public async Task<IActionResult> Create(CrudForStatics dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail is null)
                return Unauthorized("User not authenticated");


            VehicleType type = new()
            {                
                Name=dto.Name,               
            };
            await _unitOfWork.VehicleTypes.CreateAsync(type);
            _unitOfWork.Complete();
            return Ok(type);

        }

        [Authorize(Roles = "Manger")]
        [HttpPut("updateType/{id}")]
        public async Task<IActionResult> Update(int id, CrudForStatics dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail is null)
                return Unauthorized("User not authenticated");


            var type = await _unitOfWork.VehicleTypes.GetByID(v => v.Id == id);

            if (type is null)
                return NotFound($"No vehicle With ID {id}");

            type.Name = dto.Name;

            _unitOfWork.VehicleTypes.UpdateAsync(type);
            _unitOfWork.Complete();

            return Ok(type);
        }



        [HttpGet("types")]
        public async Task<IActionResult> Read()
        {
            var vehicles = await _unitOfWork.VehicleTypes.ReadAsync();
            return Ok(vehicles);
        }


        [Authorize(Roles = "Manger")]
        [HttpDelete("deleteType/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var vehicle = await _unitOfWork.VehicleTypes.GetByID(v => v.Id == id);

            if (vehicle is null)
                return NotFound($"No vehicle With ID {id}");

            _unitOfWork.VehicleTypes.DeleteAsync(vehicle);
            _unitOfWork.Complete();
            return Ok(vehicle);
        }
    }
}

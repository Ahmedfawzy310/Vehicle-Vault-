using VehicleVault.Core.Entities;

namespace VehicleVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SettingsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public SettingsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Manger")]
        [HttpPost("newState")]
        public async Task<IActionResult> CreateState(CrudForStatics dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail is null)
                return Unauthorized("User not authenticated");

            State st = new()
            {
                Name = dto.Name
            };
            await _unitOfWork.States.CreateAsync(st);
            _unitOfWork.Complete();

            return Ok(st);
        }

        [Authorize(Roles = "Manger")]
        [HttpPut("updateState/{id}")]
        public async Task<IActionResult> Update(int id, CrudForStatics dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail is null)
                return Unauthorized("User not authenticated");


            var state = await _unitOfWork.States.GetByID(v => v.Id == id);

            if (state is null)
                return NotFound($"No State With ID {id}");

            state.Name = dto.Name;

            _unitOfWork.States.UpdateAsync(state);
            _unitOfWork.Complete();

            return Ok(state);
        }

        [HttpGet("states")]
        public async Task<IActionResult> Read()
        {
            var states = await _unitOfWork.States.ReadAsync();
            return Ok(states);
        }

        [Authorize(Roles = "Manger")]
        [HttpDelete("deleteState/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var state = await _unitOfWork.States.GetByID(v => v.Id == id);

            if (state is null)
                return NotFound($"No State With ID {id}");

            _unitOfWork.States.DeleteAsync(state);
            _unitOfWork.Complete();
            return Ok(state);
        }





        [Authorize(Roles = "Manger")]
        [HttpPost("newMethod")]
        public async Task<IActionResult> CreateMethod(CrudForStatics dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail is null)
                return Unauthorized("User not authenticated");

            PaymentMethod method = new()
            {
                Name = dto.Name
            };
            await _unitOfWork.PaymentMethods.CreateAsync(method);
            _unitOfWork.Complete();

            return Ok(method);
        }

        [Authorize(Roles = "Manger")]
        [HttpPut("updateMethod/{id}")]
        public async Task<IActionResult> UpdateMethod(int id, CrudForStatics dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userEmail is null)
                return Unauthorized("User not authenticated");


            var method = await _unitOfWork.PaymentMethods.GetByID(v => v.Id == id);

            if (method is null)
                return NotFound($"No Method With ID {id}");

            method.Name = dto.Name;

            _unitOfWork.PaymentMethods.UpdateAsync(method);
            _unitOfWork.Complete();

            return Ok(method);
        }

        [HttpGet("methods")]
        public async Task<IActionResult> ReadMethods()
        {
            var methods = await _unitOfWork.PaymentMethods.ReadAsync();
            return Ok(methods);
        }

        [Authorize(Roles = "Manger")]
        [HttpDelete("deleteMehtod/{id}")]
        public async Task<IActionResult> DeleteMethod(int id)
        {
            var method = await _unitOfWork.PaymentMethods.GetByID(v => v.Id == id);

            if (method is null)
                return NotFound($"No Method With ID {id}");

            _unitOfWork.PaymentMethods.DeleteAsync(method);
            _unitOfWork.Complete();
            return Ok(method);
        }

    }
}

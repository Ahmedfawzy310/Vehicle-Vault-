namespace VehicleVault.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class FeedBacksController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FeedBacksController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        [HttpPost("addComment")]
        [Authorize]
        public async Task<IActionResult> Comment(FeedBackDto dto)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null)
                return Unauthorized();

            dto.CreatedBy = userEmail;

            if (!ModelState.IsValid)
                return BadRequest();


            await _unitOfWork.BaseFeedBacks.AddCommentAsync(dto);

            return Ok("Added");

        }

        [HttpPut("updateComment/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(int id, UpdateCommentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            if (userEmail == null)
                return Unauthorized("User not authenticated");

            dto.UpdatedBy = userEmail;

            var isUpdate = await _unitOfWork.BaseFeedBacks.UpdateComment(id, dto);

            if (!isUpdate)
                return BadRequest("Not Updated");

            return Ok("Updated");
        }

        [HttpGet("Comments")]
        public async Task<IActionResult> Comments()
        {
            return Ok(await _unitOfWork.BaseFeedBacks.ReadAsync());
        }

        [HttpDelete("DeleteComment")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _unitOfWork.BaseFeedBacks.GetByID(i => i.Id == id);
            if (comment is null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.PrimarySid)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userId == null)
                return Unauthorized("User not authenticated");

            if (userId != comment.CreatedBy || userRole != "Manger")
                return BadRequest("it is not your Comment");

            _unitOfWork.BaseFeedBacks.DeleteAsync(comment);
            _unitOfWork.Complete();

            return Ok("deleted");
        }
    }
}

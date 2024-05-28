namespace VehicleVault.Ef.Repositories
{
    public class AdminRequestsServices : BaseRepository<AdminRequest>, IAdminRequestsServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _imagePath;

        public AdminRequestsServices(UserManager<ApplicationUser> userManager, ApplicationDbContext context, IUnitOfWork unitOfWork) : base(context)
        {
            _userManager = userManager;
            _context = context;
            _unitOfWork = unitOfWork;
            _imagePath = "\\VehicleVault.Api\\Images\\";
        }

        public async Task<RequestToBeAdminDto> RequestAdminRole(RequestToBeAdminDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return new RequestToBeAdminDto { Messege = "User not found." };

            if (await _userManager.IsInRoleAsync(user, "Admin"))
                return new RequestToBeAdminDto { Messege = "You are already an admin." };

            var existingRequest = await _unitOfWork.AdminRequests.GetByID(r => r.UserId == dto.UserId && r.Status == "Pending");
            if (existingRequest != null)
                return new RequestToBeAdminDto { Messege = "Your request for the Admin role is already pending approval." };

            var request = new AdminRequest
            {
                UserId = dto.UserId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                FrontCardImg=SaveCover(dto.FrontCard),
                BackCardImg=SaveCover(dto.BackCard),                    
            };

            await _unitOfWork.AdminRequests.CreateAsync(request);
            _unitOfWork.Complete();

            return new RequestToBeAdminDto { Messege = "Your request for the Admin role has been submitted and is pending approval." };
        }
        public async Task<IEnumerable<AdminRequest>> GetPendingRequestsAsync()
        {
            return await _unitOfWork.AdminRequests.ReadAsync(r => r.Status == "Pending");
        }

        public async Task<bool> ApproveRequestAsync(int requestId)
        {

            var request = await _unitOfWork.AdminRequests.GetByID(r => r.Id == requestId);
            if (request is null)
                return false;

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null)
                return false;


            var result = await _userManager.AddToRoleAsync(user, "Admin");
            if (!result.Succeeded)
                return false;

            request.Status = "Approved";
            request.ApprovedAt = DateTime.UtcNow;
            _unitOfWork.Complete();

            return true;
        }

        public async Task<bool> RejectRequestAsync(int requestId)
        { 
            var request = await _unitOfWork.AdminRequests.GetByID(r => r.Id == requestId);
            if (request is null)
                return false;

            request.Status = "Rejected";
            request.RejectedAt = DateTime.UtcNow;
            _unitOfWork.Complete();

            return true;
        }

        private string SaveCover(IFormFile cover)
        {
            if (cover == null)
                return null;

            var coverName = $"{Guid.NewGuid()}{Path.GetExtension(cover.FileName)}";

            var path = Path.Combine(_imagePath, coverName);


            using var stream = File.Create(path);
            cover.CopyTo(stream);

            return coverName;
        }
    }
}

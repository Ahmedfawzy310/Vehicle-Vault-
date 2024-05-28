using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using VehicleVault.Core.Settings;
using VehicleVault.Ef.Data;

namespace VehicleVault.Ef.Repositories
{
    public class UnitOfWork:IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly JWT _jwt;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly EmailSettings _mailSettings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<VehicleHub> _hubContext;

        public IMailServices MailServices { get; private set; }
        public IUserServices UserServices { get; private set; }
        public IOffersServices OffersServices { get; private set; }
        public IBaseVehicles BaseVehicles { get; private set; }
        public IBaseRental BaseRentals { get; private set; }
        public IBaseRepository<Offer> Offers { get; private set; }
        public IBaseRepository<State> States { get; private set; }
        public IBaseRepository<Decoration> Decorations { get; private set; }
        public IBaseRepository<VehicleType> VehicleTypes { get; private set; }
        public IBaseRepository<Image> Images { get; private set; }
        public IBaseRepository<PaymentMethod> PaymentMethods { get; private set; }
        public IBaseRepository<AdminRequest> AdminRequests { get; private set; }
        public IAdminRequestsServices AdminRequestsServices { get; private set; }
        public IBaseFeedBacks BaseFeedBacks { get; private set; }
        public IBasePayments BasePayments { get; private set; }
        public UnitOfWork(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
                         IOptions<JWT> jwt, IOptions<EmailSettings> mailSettings, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment, IConfiguration configuration, IHubContext<VehicleHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _mailSettings = mailSettings.Value;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = this;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _hubContext = hubContext;

            MailServices = new MailServices(mailSettings, _userManager);
            UserServices = new UserServices(_userManager, _roleManager, jwt, _unitOfWork);
            OffersServices = new OffersServices(_context);
            Offers = new BaseRepository<Offer>(_context);
            States = new BaseRepository<State>(_context);
            VehicleTypes = new BaseRepository<VehicleType>(_context);
            AdminRequests = new BaseRepository<AdminRequest>(_context);
            Images = new BaseRepository<Image>(_context);
            PaymentMethods = new BaseRepository<PaymentMethod>(_context);
            Decorations = new BaseRepository<Decoration>(_context);
            BaseVehicles = new BaseVehicles(_context, _unitOfWork, _httpContextAccessor, _configuration,_hubContext);
            BaseRentals = new BaseRental(_unitOfWork, _httpContextAccessor, _context, _configuration);
            AdminRequestsServices = new AdminRequestsServices(_userManager, _context, _unitOfWork);
            BasePayments = new BasePayments(_context, _unitOfWork, _userManager, _webHostEnvironment);
            BaseFeedBacks = new BaseFeedBacks(_context, _unitOfWork);
           
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

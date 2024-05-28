namespace VehicleVault.Core.Interfaces
{
    public interface IAdminRequestsServices : IBaseRepository<AdminRequest>
    {
        Task<RequestToBeAdminDto> RequestAdminRole(RequestToBeAdminDto dto);
        Task<IEnumerable<AdminRequest>> GetPendingRequestsAsync();
        Task<bool> ApproveRequestAsync(int requestId);
        Task<bool> RejectRequestAsync(int requestId);
    }
}

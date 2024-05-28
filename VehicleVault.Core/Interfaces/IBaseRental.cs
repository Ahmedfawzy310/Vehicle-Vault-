global using VehicleVault.Core.DTOS;
global using VehicleVault.Core.Entities;

namespace VehicleVault.Core.Interfaces
{
    public interface IBaseRental : IBaseRepository<Rental>
    {
        Task<RentalDto> CreateRental(RentalDto rentalDto);
        Task<bool> CanUserAddComment(string userId, int vehicleId);
        Task<IEnumerable<DisplayRentsForAdminVehciles>> GetRentsForAdmin();
        Task<IEnumerable<DisplayRentsForUser>> GetRentsForUser();
        Task<IEnumerable<DisplayRentsForManger>> GetRentsForManger();
    }
}

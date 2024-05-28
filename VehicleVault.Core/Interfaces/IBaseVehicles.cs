using Microsoft.AspNetCore.Mvc;

namespace VehicleVault.Core.Interfaces
{
    public interface IBaseVehicles : IBaseRepository<Vehicle>
    {
        Task<Vehicle> CreateVehicleAsync([FromForm] CreateVehicleDto dto);
        Task<Vehicle> UpdateVehicleAsync(int vehicleId, UpdateVehicleDto dto);
        Task<VehicleDetailsDto> Details(int id);
        Task<IEnumerable<VehicleDetailsDto>> ReadAll(string useremail = null);
        Task UpdateVehicleAvailability();
    }
}

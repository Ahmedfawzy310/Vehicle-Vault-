using Microsoft.AspNetCore.SignalR;

namespace VehicleVault.Ef.Data
{
    public class VehicleHub : Hub
    {
        public async Task SendVehicleAvailabilityUpdate(int vehicleId, bool isAvailable)
        {
            await Clients.All.SendAsync("ReceiveVehicleAvailabilityUpdate", vehicleId, isAvailable);
        }
    }
}

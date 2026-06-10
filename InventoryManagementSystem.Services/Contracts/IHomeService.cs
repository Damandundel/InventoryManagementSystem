using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Services.Contracts
{
    public interface IHomeService
    {
        Task<DashboardServiceModel> GetDashboardDataAsync(string ownerId);
        Task<StatisticsServiceModel> GetStatisticsAsync(string ownerId);
    }
}

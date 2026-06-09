using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Services.Contracts
{
    public interface IHomeService
    {
        Task<DashboardServiceModel> GetDashboardDataAsync();
        Task<StatisticsServiceModel> GetStatisticsAsync();
    }
}

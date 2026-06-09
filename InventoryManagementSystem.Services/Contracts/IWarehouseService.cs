using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Services.Contracts
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseServiceModel>> GetAllAsync();
        Task<WarehouseServiceModel?> GetByIdAsync(int id);
        Task<int> CreateAsync(string name, string location);
        Task<bool> EditAsync(int id, string name, string location);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

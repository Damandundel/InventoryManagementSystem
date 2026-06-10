using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Services.Contracts
{
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseServiceModel>> GetAllAsync(string ownerId);
        Task<WarehouseServiceModel?> GetByIdAsync(int id, string ownerId);
        Task<int> CreateAsync(string ownerId, string name, string location);
        Task<bool> EditAsync(int id, string ownerId, string name, string location);
        Task<bool> DeleteAsync(int id, string ownerId);
        Task<bool> ExistsAsync(int id, string ownerId);
    }
}

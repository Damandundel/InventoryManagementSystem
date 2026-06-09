using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Services.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<ProductServiceModel>> GetAllAsync();
        Task<ProductServiceModel?> GetByIdAsync(int id);
        Task<int> CreateAsync(string name, string? description, decimal price, int quantity, int categoryId, int supplierId, int warehouseId);
        Task<bool> EditAsync(int id, string name, string? description, decimal price, int quantity, int categoryId, int supplierId, int warehouseId);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<ProductServiceModel>> SearchAsync(string query);
    }
}

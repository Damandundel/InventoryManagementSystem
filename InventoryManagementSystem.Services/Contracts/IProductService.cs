using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Services.Contracts
{
    public interface IProductService
    {
        Task<IEnumerable<ProductServiceModel>> GetAllAsync(string ownerId);
        Task<ProductServiceModel?> GetByIdAsync(int id, string ownerId);
        Task<IEnumerable<ProductServiceModel>> GetByCategoryAsync(int categoryId, string ownerId);
        Task<IEnumerable<ProductServiceModel>> GetBySupplierAsync(int supplierId, string ownerId);
        Task<IEnumerable<ProductServiceModel>> GetByWarehouseAsync(int warehouseId, string ownerId);
        Task<int> CreateAsync(string ownerId, string name, string? description, decimal price, int quantity, int categoryId, int supplierId, int warehouseId);
        Task<bool> EditAsync(int id, string ownerId, string name, string? description, decimal price, int quantity, int categoryId, int supplierId, int warehouseId);
        Task<bool> DeleteAsync(int id, string ownerId);
        Task<bool> ExistsAsync(int id, string ownerId);
        Task<IEnumerable<ProductServiceModel>> SearchAsync(string query, string ownerId);
    }
}

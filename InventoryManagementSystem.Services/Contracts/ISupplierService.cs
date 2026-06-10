using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Services.Contracts
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierServiceModel>> GetAllAsync(string ownerId);
        Task<SupplierServiceModel?> GetByIdAsync(int id, string ownerId);
        Task<int> CreateAsync(string ownerId, string name, string email, string phoneNumber);
        Task<bool> EditAsync(int id, string ownerId, string name, string email, string phoneNumber);
        Task<bool> DeleteAsync(int id, string ownerId);
        Task<bool> ExistsAsync(int id, string ownerId);
    }
}

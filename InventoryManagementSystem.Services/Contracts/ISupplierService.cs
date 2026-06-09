using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Services.Contracts
{
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierServiceModel>> GetAllAsync();
        Task<SupplierServiceModel?> GetByIdAsync(int id);
        Task<int> CreateAsync(string name, string email, string phoneNumber);
        Task<bool> EditAsync(int id, string name, string email, string phoneNumber);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

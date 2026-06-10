using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Services.Contracts
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryServiceModel>> GetAllAsync(string ownerId);
        Task<CategoryServiceModel?> GetByIdAsync(int id, string ownerId);
        Task<int> CreateAsync(string ownerId, string name, string? description);
        Task<bool> EditAsync(int id, string ownerId, string name, string? description);
        Task<bool> DeleteAsync(int id, string ownerId);
        Task<bool> ExistsAsync(int id, string ownerId);
    }
}

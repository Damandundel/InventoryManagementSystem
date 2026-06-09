using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Services.Contracts
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryServiceModel>> GetAllAsync();
        Task<CategoryServiceModel?> GetByIdAsync(int id);
        Task<int> CreateAsync(string name, string? description);
        Task<bool> EditAsync(int id, string name, string? description);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}

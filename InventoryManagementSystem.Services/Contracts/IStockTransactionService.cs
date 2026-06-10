using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Services.Contracts
{
    public interface IStockTransactionService
    {
        Task<IEnumerable<StockTransactionServiceModel>> GetAllAsync(string ownerId);
        Task<IEnumerable<StockTransactionServiceModel>> GetByProductIdAsync(int productId, string ownerId);
        Task<bool> AddStockAsync(int productId, int quantity, string? note, string ownerId);
        Task<bool> RemoveStockAsync(int productId, int quantity, string? note, string ownerId);
    }
}

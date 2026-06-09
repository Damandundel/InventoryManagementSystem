using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Services.Contracts
{
    public interface IStockTransactionService
    {
        Task<IEnumerable<StockTransactionServiceModel>> GetAllAsync();
        Task<IEnumerable<StockTransactionServiceModel>> GetByProductIdAsync(int productId);
        Task<bool> AddStockAsync(int productId, int quantity, string? note);
        Task<bool> RemoveStockAsync(int productId, int quantity, string? note);
    }
}

using InventoryManagementSystem.Data;
using InventoryManagementSystem.Data.Constants;
using InventoryManagementSystem.Data.Models;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services.Implementations
{
    public class StockTransactionService : IStockTransactionService
    {
        private readonly InventoryDbContext data;

        public StockTransactionService(InventoryDbContext data)
        {
            this.data = data;
        }

        public async Task<IEnumerable<StockTransactionServiceModel>> GetAllAsync(string ownerId)
        {
            return await data.StockTransactions
                .Where(st => st.Product.OwnerId == ownerId)
                .AsNoTracking()
                .OrderByDescending(st => st.CreatedOn)
                .Select(st => new StockTransactionServiceModel
                {
                    Id = st.Id,
                    ProductId = st.ProductId,
                    ProductName = st.Product.Name,
                    QuantityChanged = st.QuantityChanged,
                    Type = st.Type,
                    CreatedOn = st.CreatedOn,
                    Note = st.Note
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<StockTransactionServiceModel>> GetByProductIdAsync(int productId, string ownerId)
        {
            return await data.StockTransactions
                .Where(st => st.ProductId == productId && st.Product.OwnerId == ownerId)
                .AsNoTracking()
                .OrderByDescending(st => st.CreatedOn)
                .Select(st => new StockTransactionServiceModel
                {
                    Id = st.Id,
                    ProductId = st.ProductId,
                    ProductName = st.Product.Name,
                    QuantityChanged = st.QuantityChanged,
                    Type = st.Type,
                    CreatedOn = st.CreatedOn,
                    Note = st.Note
                })
                .ToListAsync();
        }

        public async Task<bool> AddStockAsync(int productId, int quantity, string? note, string ownerId)
        {
            var product = await data.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.OwnerId == ownerId);

            if (product == null || product.IsDeleted)
            {
                return false;
            }

            product.Quantity += quantity;

            var transaction = new StockTransaction
            {
                ProductId = productId,
                QuantityChanged = quantity,
                Type = StockTransactionTypes.Add,
                CreatedOn = DateTime.UtcNow,
                Note = note
            };

            await data.StockTransactions.AddAsync(transaction);
            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RemoveStockAsync(int productId, int quantity, string? note, string ownerId)
        {
            var product = await data.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.OwnerId == ownerId);

            if (product == null || product.IsDeleted)
            {
                return false;
            }

            if (product.Quantity < quantity)
            {
                return false;
            }

            product.Quantity -= quantity;

            var transaction = new StockTransaction
            {
                ProductId = productId,
                QuantityChanged = quantity,
                Type = StockTransactionTypes.Remove,
                CreatedOn = DateTime.UtcNow,
                Note = note
            };

            await data.StockTransactions.AddAsync(transaction);
            await data.SaveChangesAsync();

            return true;
        }
    }
}

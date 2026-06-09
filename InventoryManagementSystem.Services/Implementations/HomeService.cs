using InventoryManagementSystem.Data;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services.Implementations
{
    public class HomeService : IHomeService
    {
        private readonly InventoryDbContext data;

        public HomeService(InventoryDbContext data)
        {
            this.data = data;
        }

        public async Task<DashboardServiceModel> GetDashboardDataAsync()
        {
            var products = data.Products.Where(p => !p.IsDeleted);

            var totalProducts = await products.CountAsync();

            var dashboard = new DashboardServiceModel
            {
                TotalProducts = totalProducts,
                TotalCategories = await data.Categories.CountAsync(c => !c.IsDeleted),
                TotalSuppliers = await data.Suppliers.CountAsync(s => !s.IsDeleted),
                TotalWarehouses = await data.Warehouses.CountAsync(w => !w.IsDeleted),
                TotalStockQuantity = totalProducts > 0
                    ? await products.SumAsync(p => p.Quantity)
                    : 0,
                TotalInventoryValue = totalProducts > 0
                    ? await products.SumAsync(p => p.Price * p.Quantity)
                    : 0m,
                LowStockProducts = await products
                    .Where(p => p.Quantity < 10)
                    .AsNoTracking()
                    .Select(p => new ProductServiceModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Quantity = p.Quantity,
                        CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                        WarehouseName = p.Warehouse != null ? p.Warehouse.Name : string.Empty
                    })
                    .ToListAsync(),
                RecentTransactions = await data.StockTransactions
                    .Where(st => st.Product != null && !st.Product.IsDeleted)
                    .AsNoTracking()
                    .OrderByDescending(st => st.CreatedOn)
                    .Take(10)
                    .Select(st => new StockTransactionServiceModel
                    {
                        Id = st.Id,
                        ProductId = st.ProductId,
                        ProductName = st.Product != null ? st.Product.Name : string.Empty,
                        QuantityChanged = st.QuantityChanged,
                        Type = st.Type,
                        CreatedOn = st.CreatedOn,
                        Note = st.Note
                    })
                    .ToListAsync()
            };

            return dashboard;
        }

        public async Task<StatisticsServiceModel> GetStatisticsAsync()
        {
            var products = data.Products.Where(p => !p.IsDeleted);

            var byCategory = await data.Categories
                .Where(c => !c.IsDeleted)
                .AsNoTracking()
                .Select(c => new CategoryBreakdownModel
                {
                    Name = c.Name,
                    ProductCount = c.Products.Count(p => !p.IsDeleted),
                    TotalQuantity = c.Products.Where(p => !p.IsDeleted).Sum(p => p.Quantity),
                    TotalValue = c.Products.Where(p => !p.IsDeleted).Sum(p => p.Price * p.Quantity)
                })
                .OrderByDescending(c => c.TotalValue)
                .ToListAsync();

            var byWarehouse = await data.Warehouses
                .Where(w => !w.IsDeleted)
                .AsNoTracking()
                .Select(w => new WarehouseBreakdownModel
                {
                    Name = w.Name,
                    Location = w.Location,
                    ProductCount = w.Products.Count(p => !p.IsDeleted),
                    TotalQuantity = w.Products.Where(p => !p.IsDeleted).Sum(p => p.Quantity),
                    TotalValue = w.Products.Where(p => !p.IsDeleted).Sum(p => p.Price * p.Quantity)
                })
                .OrderByDescending(w => w.TotalValue)
                .ToListAsync();

            var topProducts = await products
                .AsNoTracking()
                .Select(p => new TopProductModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    TotalValue = p.Price * p.Quantity
                })
                .OrderByDescending(p => p.TotalValue)
                .Take(10)
                .ToListAsync();

            return new StatisticsServiceModel
            {
                ByCategory = byCategory,
                ByWarehouse = byWarehouse,
                TopProductsByValue = topProducts
            };
        }
    }
}

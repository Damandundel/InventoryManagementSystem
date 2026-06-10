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

        public async Task<DashboardServiceModel> GetDashboardDataAsync(string ownerId)
        {
            var products = data.Products.Where(p => !p.IsDeleted && p.OwnerId == ownerId);

            var totalProducts = await products.CountAsync();

            // SQLite cannot aggregate decimals in SQL, so pull the price/quantity
            // pairs and compute the monetary total in memory.
            var priceQuantities = await products
                .AsNoTracking()
                .Select(p => new { p.Price, p.Quantity })
                .ToListAsync();

            var dashboard = new DashboardServiceModel
            {
                TotalProducts = totalProducts,
                TotalCategories = await data.Categories.CountAsync(c => !c.IsDeleted && c.OwnerId == ownerId),
                TotalSuppliers = await data.Suppliers.CountAsync(s => !s.IsDeleted && s.OwnerId == ownerId),
                TotalWarehouses = await data.Warehouses.CountAsync(w => !w.IsDeleted && w.OwnerId == ownerId),
                TotalStockQuantity = priceQuantities.Sum(p => p.Quantity),
                TotalInventoryValue = priceQuantities.Sum(p => p.Price * p.Quantity),
                LowStockProducts = await products
                    .Where(p => p.Quantity < 10)
                    .AsNoTracking()
                    .OrderBy(p => p.Quantity)
                    .Select(p => new ProductServiceModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Price = p.Price,
                        Quantity = p.Quantity,
                        CategoryId = p.CategoryId,
                        CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                        WarehouseId = p.WarehouseId,
                        WarehouseName = p.Warehouse != null ? p.Warehouse.Name : string.Empty
                    })
                    .ToListAsync(),
                RecentTransactions = await data.StockTransactions
                    .Where(st => st.Product != null && !st.Product.IsDeleted && st.Product.OwnerId == ownerId)
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

        public async Task<StatisticsServiceModel> GetStatisticsAsync(string ownerId)
        {
            // Monetary aggregation/ordering is done in memory so the queries work
            // on any provider (SQLite cannot aggregate or order decimals in SQL).
            var byCategory = (await data.Categories
                .Where(c => !c.IsDeleted && c.OwnerId == ownerId)
                .AsNoTracking()
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    Products = c.Products.Where(p => !p.IsDeleted)
                        .Select(p => new { p.Price, p.Quantity })
                })
                .ToListAsync())
                .Select(c => new CategoryBreakdownModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    ProductCount = c.Products.Count(),
                    TotalQuantity = c.Products.Sum(p => p.Quantity),
                    TotalValue = c.Products.Sum(p => p.Price * p.Quantity)
                })
                .OrderByDescending(c => c.TotalValue)
                .ToList();

            var byWarehouse = (await data.Warehouses
                .Where(w => !w.IsDeleted && w.OwnerId == ownerId)
                .AsNoTracking()
                .Select(w => new
                {
                    w.Id,
                    w.Name,
                    w.Location,
                    Products = w.Products.Where(p => !p.IsDeleted)
                        .Select(p => new { p.Price, p.Quantity })
                })
                .ToListAsync())
                .Select(w => new WarehouseBreakdownModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Location = w.Location,
                    ProductCount = w.Products.Count(),
                    TotalQuantity = w.Products.Sum(p => p.Quantity),
                    TotalValue = w.Products.Sum(p => p.Price * p.Quantity)
                })
                .OrderByDescending(w => w.TotalValue)
                .ToList();

            var topProducts = (await data.Products
                .Where(p => !p.IsDeleted && p.OwnerId == ownerId)
                .AsNoTracking()
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                    p.Price,
                    p.Quantity
                })
                .ToListAsync())
                .Select(p => new TopProductModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.CategoryName,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    TotalValue = p.Price * p.Quantity
                })
                .OrderByDescending(p => p.TotalValue)
                .Take(10)
                .ToList();

            return new StatisticsServiceModel
            {
                ByCategory = byCategory,
                ByWarehouse = byWarehouse,
                TopProductsByValue = topProducts
            };
        }
    }
}

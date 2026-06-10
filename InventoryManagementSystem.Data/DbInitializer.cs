using InventoryManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Data
{
    /// <summary>
    /// Seeds the example demo catalog (categories, suppliers, warehouses, products)
    /// for a single owner. Used to give the example admin account ready-made data,
    /// while newly registered users start with an empty inventory.
    /// </summary>
    public static class DbInitializer
    {
        public static async Task SeedDemoDataAsync(InventoryDbContext data, string ownerId)
        {
            // Only seed once for this owner.
            if (await data.Products.AnyAsync(p => p.OwnerId == ownerId))
            {
                return;
            }

            var electronics = new Category { Name = "Electronics", Description = "Electronic devices and components", OwnerId = ownerId };
            var furniture = new Category { Name = "Furniture", Description = "Office and home furniture", OwnerId = ownerId };
            var clothing = new Category { Name = "Clothing", Description = "Apparel and accessories", OwnerId = ownerId };

            var techSupply = new Supplier { Name = "TechSupply Co.", Email = "contact@techsupply.com", PhoneNumber = "+1234567890", OwnerId = ownerId };
            var globalGoods = new Supplier { Name = "Global Goods Ltd.", Email = "info@globalgoods.com", PhoneNumber = "+0987654321", OwnerId = ownerId };
            var primeMaterials = new Supplier { Name = "Prime Materials", Email = "sales@primematerials.com", PhoneNumber = "+1122334455", OwnerId = ownerId };

            var mainWarehouse = new Warehouse { Name = "Main Warehouse", Location = "123 Industrial Ave, New York, NY", OwnerId = ownerId };
            var eastStorage = new Warehouse { Name = "East Storage", Location = "456 Commerce Blvd, Boston, MA", OwnerId = ownerId };
            var westDepot = new Warehouse { Name = "West Depot", Location = "789 Trade St, Los Angeles, CA", OwnerId = ownerId };

            await data.Categories.AddRangeAsync(electronics, furniture, clothing);
            await data.Suppliers.AddRangeAsync(techSupply, globalGoods, primeMaterials);
            await data.Warehouses.AddRangeAsync(mainWarehouse, eastStorage, westDepot);
            await data.SaveChangesAsync();

            var products = new[]
            {
                new Product { Name = "Laptop Pro 15", Description = "High-performance laptop with 16GB RAM", Price = 1299.99m, Quantity = 50, Category = electronics, Supplier = techSupply, Warehouse = mainWarehouse, OwnerId = ownerId },
                new Product { Name = "Ergonomic Office Chair", Description = "Adjustable office chair with lumbar support", Price = 349.50m, Quantity = 120, Category = furniture, Supplier = globalGoods, Warehouse = eastStorage, OwnerId = ownerId },
                new Product { Name = "Wireless Mouse", Description = "Bluetooth wireless mouse with ergonomic design", Price = 29.99m, Quantity = 500, Category = electronics, Supplier = techSupply, Warehouse = mainWarehouse, OwnerId = ownerId },
                new Product { Name = "Standing Desk", Description = "Electric height-adjustable standing desk", Price = 599.00m, Quantity = 30, Category = furniture, Supplier = globalGoods, Warehouse = westDepot, OwnerId = ownerId },
                new Product { Name = "Safety Vest", Description = "High-visibility safety vest", Price = 19.99m, Quantity = 200, Category = clothing, Supplier = primeMaterials, Warehouse = westDepot, OwnerId = ownerId }
            };

            await data.Products.AddRangeAsync(products);
            await data.SaveChangesAsync();
        }
    }
}

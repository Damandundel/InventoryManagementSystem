using InventoryManagementSystem.Data;
using InventoryManagementSystem.Data.Models;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly InventoryDbContext data;

        public ProductService(InventoryDbContext data)
        {
            this.data = data;
        }

        public async Task<IEnumerable<ProductServiceModel>> GetAllAsync()
        {
            return await data.Products
                .Where(p => !p.IsDeleted)
                .AsNoTracking()
                .Select(p => new ProductServiceModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    SupplierId = p.SupplierId,
                    SupplierName = p.Supplier.Name,
                    WarehouseId = p.WarehouseId,
                    WarehouseName = p.Warehouse.Name
                })
                .ToListAsync();
        }

        public async Task<ProductServiceModel?> GetByIdAsync(int id)
        {
            return await data.Products
                .Where(p => !p.IsDeleted && p.Id == id)
                .AsNoTracking()
                .Select(p => new ProductServiceModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    SupplierId = p.SupplierId,
                    SupplierName = p.Supplier.Name,
                    WarehouseId = p.WarehouseId,
                    WarehouseName = p.Warehouse.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(string name, string? description, decimal price, int quantity, int categoryId, int supplierId, int warehouseId)
        {
            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                Quantity = quantity,
                CategoryId = categoryId,
                SupplierId = supplierId,
                WarehouseId = warehouseId
            };

            await data.Products.AddAsync(product);
            await data.SaveChangesAsync();

            return product.Id;
        }

        public async Task<bool> EditAsync(int id, string name, string? description, decimal price, int quantity, int categoryId, int supplierId, int warehouseId)
        {
            var product = await data.Products.FindAsync(id);

            if (product == null || product.IsDeleted)
            {
                return false;
            }

            product.Name = name;
            product.Description = description;
            product.Price = price;
            product.Quantity = quantity;
            product.CategoryId = categoryId;
            product.SupplierId = supplierId;
            product.WarehouseId = warehouseId;

            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await data.Products.FindAsync(id);

            if (product == null || product.IsDeleted)
            {
                return false;
            }

            product.IsDeleted = true;
            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await data.Products
                .AsNoTracking()
                .AnyAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<IEnumerable<ProductServiceModel>> SearchAsync(string query)
        {
            return await data.Products
                .Where(p => !p.IsDeleted &&
                    (p.Name.Contains(query) || (p.Description != null && p.Description.Contains(query))))
                .AsNoTracking()
                .Select(p => new ProductServiceModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    SupplierId = p.SupplierId,
                    SupplierName = p.Supplier.Name,
                    WarehouseId = p.WarehouseId,
                    WarehouseName = p.Warehouse.Name
                })
                .ToListAsync();
        }
    }
}

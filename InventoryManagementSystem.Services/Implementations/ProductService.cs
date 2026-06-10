using System.Linq.Expressions;
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

        private static readonly Expression<Func<Product, ProductServiceModel>> ToServiceModel =
            p => new ProductServiceModel
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
            };

        public async Task<IEnumerable<ProductServiceModel>> GetAllAsync(string ownerId)
        {
            return await data.Products
                .Where(p => !p.IsDeleted && p.OwnerId == ownerId)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(ToServiceModel)
                .ToListAsync();
        }

        public async Task<ProductServiceModel?> GetByIdAsync(int id, string ownerId)
        {
            return await data.Products
                .Where(p => !p.IsDeleted && p.Id == id && p.OwnerId == ownerId)
                .AsNoTracking()
                .Select(ToServiceModel)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProductServiceModel>> GetByCategoryAsync(int categoryId, string ownerId)
        {
            return await data.Products
                .Where(p => !p.IsDeleted && p.OwnerId == ownerId && p.CategoryId == categoryId)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(ToServiceModel)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductServiceModel>> GetBySupplierAsync(int supplierId, string ownerId)
        {
            return await data.Products
                .Where(p => !p.IsDeleted && p.OwnerId == ownerId && p.SupplierId == supplierId)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(ToServiceModel)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductServiceModel>> GetByWarehouseAsync(int warehouseId, string ownerId)
        {
            return await data.Products
                .Where(p => !p.IsDeleted && p.OwnerId == ownerId && p.WarehouseId == warehouseId)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Select(ToServiceModel)
                .ToListAsync();
        }

        public async Task<int> CreateAsync(string ownerId, string name, string? description, decimal price, int quantity, int categoryId, int supplierId, int warehouseId)
        {
            var product = new Product
            {
                Name = name,
                Description = description,
                Price = price,
                Quantity = quantity,
                CategoryId = categoryId,
                SupplierId = supplierId,
                WarehouseId = warehouseId,
                OwnerId = ownerId
            };

            await data.Products.AddAsync(product);
            await data.SaveChangesAsync();

            return product.Id;
        }

        public async Task<bool> EditAsync(int id, string ownerId, string name, string? description, decimal price, int quantity, int categoryId, int supplierId, int warehouseId)
        {
            var product = await data.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId);

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

        public async Task<bool> DeleteAsync(int id, string ownerId)
        {
            var product = await data.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.OwnerId == ownerId);

            if (product == null || product.IsDeleted)
            {
                return false;
            }

            product.IsDeleted = true;
            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id, string ownerId)
        {
            return await data.Products
                .AsNoTracking()
                .AnyAsync(p => p.Id == id && p.OwnerId == ownerId && !p.IsDeleted);
        }

        public async Task<IEnumerable<ProductServiceModel>> SearchAsync(string query, string ownerId)
        {
            return await data.Products
                .Where(p => !p.IsDeleted && p.OwnerId == ownerId &&
                    (p.Name.Contains(query) || (p.Description != null && p.Description.Contains(query))))
                .AsNoTracking()
                .Select(ToServiceModel)
                .ToListAsync();
        }
    }
}

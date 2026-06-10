using InventoryManagementSystem.Data;
using InventoryManagementSystem.Data.Models;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services.Implementations
{
    public class WarehouseService : IWarehouseService
    {
        private readonly InventoryDbContext data;

        public WarehouseService(InventoryDbContext data)
        {
            this.data = data;
        }

        public async Task<IEnumerable<WarehouseServiceModel>> GetAllAsync(string ownerId)
        {
            return await data.Warehouses
                .Where(w => !w.IsDeleted && w.OwnerId == ownerId)
                .AsNoTracking()
                .OrderBy(w => w.Name)
                .Select(w => new WarehouseServiceModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Location = w.Location,
                    ProductCount = w.Products.Count(p => !p.IsDeleted)
                })
                .ToListAsync();
        }

        public async Task<WarehouseServiceModel?> GetByIdAsync(int id, string ownerId)
        {
            return await data.Warehouses
                .Where(w => !w.IsDeleted && w.Id == id && w.OwnerId == ownerId)
                .AsNoTracking()
                .Select(w => new WarehouseServiceModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Location = w.Location,
                    ProductCount = w.Products.Count(p => !p.IsDeleted)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(string ownerId, string name, string location)
        {
            var warehouse = new Warehouse
            {
                Name = name,
                Location = location,
                OwnerId = ownerId
            };

            await data.Warehouses.AddAsync(warehouse);
            await data.SaveChangesAsync();

            return warehouse.Id;
        }

        public async Task<bool> EditAsync(int id, string ownerId, string name, string location)
        {
            var warehouse = await data.Warehouses
                .FirstOrDefaultAsync(w => w.Id == id && w.OwnerId == ownerId);

            if (warehouse == null || warehouse.IsDeleted)
            {
                return false;
            }

            warehouse.Name = name;
            warehouse.Location = location;

            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id, string ownerId)
        {
            var warehouse = await data.Warehouses
                .FirstOrDefaultAsync(w => w.Id == id && w.OwnerId == ownerId);

            if (warehouse == null || warehouse.IsDeleted)
            {
                return false;
            }

            warehouse.IsDeleted = true;
            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id, string ownerId)
        {
            return await data.Warehouses
                .AsNoTracking()
                .AnyAsync(w => w.Id == id && w.OwnerId == ownerId && !w.IsDeleted);
        }
    }
}

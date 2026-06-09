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

        public async Task<IEnumerable<WarehouseServiceModel>> GetAllAsync()
        {
            return await data.Warehouses
                .Where(w => !w.IsDeleted)
                .AsNoTracking()
                .Select(w => new WarehouseServiceModel
                {
                    Id = w.Id,
                    Name = w.Name,
                    Location = w.Location,
                    ProductCount = w.Products.Count(p => !p.IsDeleted)
                })
                .ToListAsync();
        }

        public async Task<WarehouseServiceModel?> GetByIdAsync(int id)
        {
            return await data.Warehouses
                .Where(w => !w.IsDeleted && w.Id == id)
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

        public async Task<int> CreateAsync(string name, string location)
        {
            var warehouse = new Warehouse
            {
                Name = name,
                Location = location
            };

            await data.Warehouses.AddAsync(warehouse);
            await data.SaveChangesAsync();

            return warehouse.Id;
        }

        public async Task<bool> EditAsync(int id, string name, string location)
        {
            var warehouse = await data.Warehouses.FindAsync(id);

            if (warehouse == null || warehouse.IsDeleted)
            {
                return false;
            }

            warehouse.Name = name;
            warehouse.Location = location;

            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var warehouse = await data.Warehouses.FindAsync(id);

            if (warehouse == null || warehouse.IsDeleted)
            {
                return false;
            }

            warehouse.IsDeleted = true;
            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await data.Warehouses
                .AsNoTracking()
                .AnyAsync(w => w.Id == id && !w.IsDeleted);
        }
    }
}

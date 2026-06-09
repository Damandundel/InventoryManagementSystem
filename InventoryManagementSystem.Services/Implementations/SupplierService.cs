using InventoryManagementSystem.Data;
using InventoryManagementSystem.Data.Models;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly InventoryDbContext data;

        public SupplierService(InventoryDbContext data)
        {
            this.data = data;
        }

        public async Task<IEnumerable<SupplierServiceModel>> GetAllAsync()
        {
            return await data.Suppliers
                .Where(s => !s.IsDeleted)
                .AsNoTracking()
                .Select(s => new SupplierServiceModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    ProductCount = s.Products.Count(p => !p.IsDeleted)
                })
                .ToListAsync();
        }

        public async Task<SupplierServiceModel?> GetByIdAsync(int id)
        {
            return await data.Suppliers
                .Where(s => !s.IsDeleted && s.Id == id)
                .AsNoTracking()
                .Select(s => new SupplierServiceModel
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,
                    PhoneNumber = s.PhoneNumber,
                    ProductCount = s.Products.Count(p => !p.IsDeleted)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(string name, string email, string phoneNumber)
        {
            var supplier = new Supplier
            {
                Name = name,
                Email = email,
                PhoneNumber = phoneNumber
            };

            await data.Suppliers.AddAsync(supplier);
            await data.SaveChangesAsync();

            return supplier.Id;
        }

        public async Task<bool> EditAsync(int id, string name, string email, string phoneNumber)
        {
            var supplier = await data.Suppliers.FindAsync(id);

            if (supplier == null || supplier.IsDeleted)
            {
                return false;
            }

            supplier.Name = name;
            supplier.Email = email;
            supplier.PhoneNumber = phoneNumber;

            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var supplier = await data.Suppliers.FindAsync(id);

            if (supplier == null || supplier.IsDeleted)
            {
                return false;
            }

            supplier.IsDeleted = true;
            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await data.Suppliers
                .AsNoTracking()
                .AnyAsync(s => s.Id == id && !s.IsDeleted);
        }
    }
}

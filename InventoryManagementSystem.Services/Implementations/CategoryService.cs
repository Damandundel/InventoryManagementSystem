using InventoryManagementSystem.Data;
using InventoryManagementSystem.Data.Models;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly InventoryDbContext data;

        public CategoryService(InventoryDbContext data)
        {
            this.data = data;
        }

        public async Task<IEnumerable<CategoryServiceModel>> GetAllAsync()
        {
            return await data.Categories
                .Where(c => !c.IsDeleted)
                .AsNoTracking()
                .Select(c => new CategoryServiceModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.Products.Count(p => !p.IsDeleted)
                })
                .ToListAsync();
        }

        public async Task<CategoryServiceModel?> GetByIdAsync(int id)
        {
            return await data.Categories
                .Where(c => !c.IsDeleted && c.Id == id)
                .AsNoTracking()
                .Select(c => new CategoryServiceModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    ProductCount = c.Products.Count(p => !p.IsDeleted)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<int> CreateAsync(string name, string? description)
        {
            var category = new Category
            {
                Name = name,
                Description = description
            };

            await data.Categories.AddAsync(category);
            await data.SaveChangesAsync();

            return category.Id;
        }

        public async Task<bool> EditAsync(int id, string name, string? description)
        {
            var category = await data.Categories.FindAsync(id);

            if (category == null || category.IsDeleted)
            {
                return false;
            }

            category.Name = name;
            category.Description = description;

            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await data.Categories.FindAsync(id);

            if (category == null || category.IsDeleted)
            {
                return false;
            }

            category.IsDeleted = true;
            await data.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await data.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Id == id && !c.IsDeleted);
        }
    }
}

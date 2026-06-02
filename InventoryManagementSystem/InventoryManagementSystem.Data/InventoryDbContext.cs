using InventoryManagementSystem.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Data
{
    public class InventoryDbContext : IdentityDbContext<ApplicationUser>
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Warehouse> Warehouses { get; set; } = null!;
        public DbSet<StockTransaction> StockTransactions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Product>()
                .HasOne(p => p.Warehouse)
                .WithMany(w => w.Products)
                .HasForeignKey(p => p.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StockTransaction>()
                .HasOne(st => st.Product)
                .WithMany(p => p.StockTransactions)
                .HasForeignKey(st => st.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            SeedData(builder);
        }

        private void SeedData(ModelBuilder builder)
        {
            builder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", Description = "Electronic devices and components" },
                new Category { Id = 2, Name = "Furniture", Description = "Office and home furniture" },
                new Category { Id = 3, Name = "Clothing", Description = "Apparel and accessories" }
            );

            builder.Entity<Supplier>().HasData(
                new Supplier { Id = 1, Name = "TechSupply Co.", Email = "contact@techsupply.com", PhoneNumber = "+1234567890" },
                new Supplier { Id = 2, Name = "Global Goods Ltd.", Email = "info@globalgoods.com", PhoneNumber = "+0987654321" },
                new Supplier { Id = 3, Name = "Prime Materials", Email = "sales@primematerials.com", PhoneNumber = "+1122334455" }
            );

            builder.Entity<Warehouse>().HasData(
                new Warehouse { Id = 1, Name = "Main Warehouse", Location = "123 Industrial Ave, New York, NY" },
                new Warehouse { Id = 2, Name = "East Storage", Location = "456 Commerce Blvd, Boston, MA" },
                new Warehouse { Id = 3, Name = "West Depot", Location = "789 Trade St, Los Angeles, CA" }
            );

            builder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Laptop Pro 15", Description = "High-performance laptop with 16GB RAM", Price = 1299.99m, Quantity = 50, CategoryId = 1, SupplierId = 1, WarehouseId = 1 },
                new Product { Id = 2, Name = "Ergonomic Office Chair", Description = "Adjustable office chair with lumbar support", Price = 349.50m, Quantity = 120, CategoryId = 2, SupplierId = 2, WarehouseId = 2 },
                new Product { Id = 3, Name = "Wireless Mouse", Description = "Bluetooth wireless mouse with ergonomic design", Price = 29.99m, Quantity = 500, CategoryId = 1, SupplierId = 1, WarehouseId = 1 },
                new Product { Id = 4, Name = "Standing Desk", Description = "Electric height-adjustable standing desk", Price = 599.00m, Quantity = 30, CategoryId = 2, SupplierId = 2, WarehouseId = 3 },
                new Product { Id = 5, Name = "Safety Vest", Description = "High-visibility safety vest", Price = 19.99m, Quantity = 200, CategoryId = 3, SupplierId = 3, WarehouseId = 3 }
            );
        }
    }
}

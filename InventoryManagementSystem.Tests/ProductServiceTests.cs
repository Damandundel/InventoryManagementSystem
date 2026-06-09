using InventoryManagementSystem.Data;
using InventoryManagementSystem.Data.Models;
using InventoryManagementSystem.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace InventoryManagementSystem.Tests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private InventoryDbContext data = null!;
        private ProductService productService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            data = new InventoryDbContext(options);

            data.Categories.Add(new Category { Id = 1, Name = "Electronics" });
            data.Suppliers.Add(new Supplier { Id = 1, Name = "TestSupplier", Email = "test@test.com", PhoneNumber = "123" });
            data.Warehouses.Add(new Warehouse { Id = 1, Name = "TestWarehouse", Location = "TestLocation" });
            data.SaveChanges();

            productService = new ProductService(data);
        }

        [TearDown]
        public void TearDown()
        {
            data.Dispose();
        }

        [Test]
        public async Task CreateAsync_ShouldCreateProduct()
        {
            var id = await productService.CreateAsync("Test Product", "Description", 10.99m, 100, 1, 1, 1);

            Assert.That(id, Is.GreaterThan(0));

            var product = await data.Products.FindAsync(id);
            Assert.That(product, Is.Not.Null);
            Assert.That(product!.Name, Is.EqualTo("Test Product"));
            Assert.That(product.Price, Is.EqualTo(10.99m));
            Assert.That(product.Quantity, Is.EqualTo(100));
        }

        [Test]
        public async Task EditAsync_ShouldEditProduct()
        {
            var id = await productService.CreateAsync("Original", "Desc", 5.00m, 50, 1, 1, 1);

            var result = await productService.EditAsync(id, "Updated", "New Desc", 15.00m, 75, 1, 1, 1);

            Assert.That(result, Is.True);

            var product = await data.Products.FindAsync(id);
            Assert.That(product!.Name, Is.EqualTo("Updated"));
            Assert.That(product.Price, Is.EqualTo(15.00m));
            Assert.That(product.Quantity, Is.EqualTo(75));
        }

        [Test]
        public async Task EditAsync_ShouldReturnFalse_WhenProductNotFound()
        {
            var result = await productService.EditAsync(999, "Name", "Desc", 1m, 1, 1, 1, 1);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteAsync_ShouldSoftDeleteProduct()
        {
            var id = await productService.CreateAsync("ToDelete", "Desc", 5.00m, 10, 1, 1, 1);

            var result = await productService.DeleteAsync(id);

            Assert.That(result, Is.True);

            var product = await data.Products.FindAsync(id);
            Assert.That(product!.IsDeleted, Is.True);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenProductNotFound()
        {
            var result = await productService.DeleteAsync(999);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnOnlyNonDeletedProducts()
        {
            await productService.CreateAsync("Active1", null, 5m, 10, 1, 1, 1);
            await productService.CreateAsync("Active2", null, 10m, 20, 1, 1, 1);
            var deletedId = await productService.CreateAsync("Deleted", null, 15m, 30, 1, 1, 1);

            await productService.DeleteAsync(deletedId);

            var products = await productService.GetAllAsync();

            Assert.That(products.Count(), Is.EqualTo(2));
            Assert.That(products.Any(p => p.Name == "Deleted"), Is.False);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalse_ForDeletedProduct()
        {
            var id = await productService.CreateAsync("Product", null, 5m, 10, 1, 1, 1);
            await productService.DeleteAsync(id);

            var exists = await productService.ExistsAsync(id);

            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task SearchAsync_ShouldFindProductsByName()
        {
            await productService.CreateAsync("Laptop Pro", null, 999m, 5, 1, 1, 1);
            await productService.CreateAsync("Mouse Wireless", null, 29m, 100, 1, 1, 1);

            var results = await productService.SearchAsync("Laptop");

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First().Name, Is.EqualTo("Laptop Pro"));
        }
    }
}

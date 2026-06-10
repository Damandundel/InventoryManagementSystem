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
        private const string OwnerId = "owner-1";
        private const string OtherOwnerId = "owner-2";

        private InventoryDbContext data = null!;
        private ProductService productService = null!;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            data = new InventoryDbContext(options);

            data.Categories.Add(new Category { Id = 1, Name = "Electronics", OwnerId = OwnerId });
            data.Categories.Add(new Category { Id = 2, Name = "Furniture", OwnerId = OwnerId });
            data.Suppliers.Add(new Supplier { Id = 1, Name = "TestSupplier", Email = "test@test.com", PhoneNumber = "123", OwnerId = OwnerId });
            data.Suppliers.Add(new Supplier { Id = 2, Name = "OtherSupplier", Email = "other@test.com", PhoneNumber = "456", OwnerId = OwnerId });
            data.Warehouses.Add(new Warehouse { Id = 1, Name = "TestWarehouse", Location = "TestLocation", OwnerId = OwnerId });
            data.Warehouses.Add(new Warehouse { Id = 2, Name = "OtherWarehouse", Location = "OtherLocation", OwnerId = OwnerId });
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
            var id = await productService.CreateAsync(OwnerId, "Test Product", "Description", 10.99m, 100, 1, 1, 1);

            Assert.That(id, Is.GreaterThan(0));

            var product = await data.Products.FindAsync(id);
            Assert.That(product, Is.Not.Null);
            Assert.That(product!.Name, Is.EqualTo("Test Product"));
            Assert.That(product.Price, Is.EqualTo(10.99m));
            Assert.That(product.Quantity, Is.EqualTo(100));
            Assert.That(product.OwnerId, Is.EqualTo(OwnerId));
        }

        [Test]
        public async Task EditAsync_ShouldEditProduct()
        {
            var id = await productService.CreateAsync(OwnerId, "Original", "Desc", 5.00m, 50, 1, 1, 1);

            var result = await productService.EditAsync(id, OwnerId, "Updated", "New Desc", 15.00m, 75, 1, 1, 1);

            Assert.That(result, Is.True);

            var product = await data.Products.FindAsync(id);
            Assert.That(product!.Name, Is.EqualTo("Updated"));
            Assert.That(product.Price, Is.EqualTo(15.00m));
            Assert.That(product.Quantity, Is.EqualTo(75));
        }

        [Test]
        public async Task EditAsync_ShouldReturnFalse_WhenProductNotFound()
        {
            var result = await productService.EditAsync(999, OwnerId, "Name", "Desc", 1m, 1, 1, 1, 1);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteAsync_ShouldSoftDeleteProduct()
        {
            var id = await productService.CreateAsync(OwnerId, "ToDelete", "Desc", 5.00m, 10, 1, 1, 1);

            var result = await productService.DeleteAsync(id, OwnerId);

            Assert.That(result, Is.True);

            var product = await data.Products.FindAsync(id);
            Assert.That(product!.IsDeleted, Is.True);
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenProductNotFound()
        {
            var result = await productService.DeleteAsync(999, OwnerId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnOnlyNonDeletedProducts()
        {
            await productService.CreateAsync(OwnerId, "Active1", null, 5m, 10, 1, 1, 1);
            await productService.CreateAsync(OwnerId, "Active2", null, 10m, 20, 1, 1, 1);
            var deletedId = await productService.CreateAsync(OwnerId, "Deleted", null, 15m, 30, 1, 1, 1);

            await productService.DeleteAsync(deletedId, OwnerId);

            var products = await productService.GetAllAsync(OwnerId);

            Assert.That(products.Count(), Is.EqualTo(2));
            Assert.That(products.Any(p => p.Name == "Deleted"), Is.False);
        }

        [Test]
        public async Task ExistsAsync_ShouldReturnFalse_ForDeletedProduct()
        {
            var id = await productService.CreateAsync(OwnerId, "Product", null, 5m, 10, 1, 1, 1);
            await productService.DeleteAsync(id, OwnerId);

            var exists = await productService.ExistsAsync(id, OwnerId);

            Assert.That(exists, Is.False);
        }

        [Test]
        public async Task SearchAsync_ShouldFindProductsByName()
        {
            await productService.CreateAsync(OwnerId, "Laptop Pro", null, 999m, 5, 1, 1, 1);
            await productService.CreateAsync(OwnerId, "Mouse Wireless", null, 29m, 100, 1, 1, 1);

            var results = await productService.SearchAsync("Laptop", OwnerId);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First().Name, Is.EqualTo("Laptop Pro"));
        }

        [Test]
        public async Task GetByCategoryAsync_ShouldReturnOnlyMatchingCategory()
        {
            await productService.CreateAsync(OwnerId, "Laptop", null, 999m, 5, 1, 1, 1);
            await productService.CreateAsync(OwnerId, "Phone", null, 599m, 10, 1, 1, 1);
            await productService.CreateAsync(OwnerId, "Desk", null, 199m, 3, 2, 1, 1);

            var results = await productService.GetByCategoryAsync(1, OwnerId);

            Assert.That(results.Count(), Is.EqualTo(2));
            Assert.That(results.All(p => p.CategoryId == 1), Is.True);
        }

        [Test]
        public async Task GetByCategoryAsync_ShouldExcludeDeletedProducts()
        {
            await productService.CreateAsync(OwnerId, "Active", null, 10m, 5, 1, 1, 1);
            var deletedId = await productService.CreateAsync(OwnerId, "Deleted", null, 10m, 5, 1, 1, 1);
            await productService.DeleteAsync(deletedId, OwnerId);

            var results = await productService.GetByCategoryAsync(1, OwnerId);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.Any(p => p.Name == "Deleted"), Is.False);
        }

        [Test]
        public async Task GetBySupplierAsync_ShouldReturnOnlyMatchingSupplier()
        {
            await productService.CreateAsync(OwnerId, "From S1", null, 10m, 5, 1, 1, 1);
            await productService.CreateAsync(OwnerId, "From S2", null, 10m, 5, 1, 2, 1);

            var results = await productService.GetBySupplierAsync(2, OwnerId);

            Assert.That(results.Count(), Is.EqualTo(1));
            Assert.That(results.First().Name, Is.EqualTo("From S2"));
            Assert.That(results.First().SupplierId, Is.EqualTo(2));
        }

        [Test]
        public async Task GetByWarehouseAsync_ShouldReturnOnlyMatchingWarehouse()
        {
            await productService.CreateAsync(OwnerId, "In W1", null, 10m, 5, 1, 1, 1);
            await productService.CreateAsync(OwnerId, "In W2", null, 10m, 5, 1, 1, 2);
            await productService.CreateAsync(OwnerId, "Also W2", null, 10m, 5, 1, 1, 2);

            var results = await productService.GetByWarehouseAsync(2, OwnerId);

            Assert.That(results.Count(), Is.EqualTo(2));
            Assert.That(results.All(p => p.WarehouseId == 2), Is.True);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnOnlyOwnersProducts()
        {
            await productService.CreateAsync(OwnerId, "Mine", null, 10m, 5, 1, 1, 1);
            await productService.CreateAsync(OtherOwnerId, "Theirs", null, 10m, 5, 1, 1, 1);

            var mine = await productService.GetAllAsync(OwnerId);

            Assert.That(mine.Count(), Is.EqualTo(1));
            Assert.That(mine.First().Name, Is.EqualTo("Mine"));
        }

        [Test]
        public async Task GetByIdAsync_ShouldNotReturnAnotherOwnersProduct()
        {
            var id = await productService.CreateAsync(OtherOwnerId, "Theirs", null, 10m, 5, 1, 1, 1);

            var result = await productService.GetByIdAsync(id, OwnerId);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task EditAsync_ShouldReturnFalse_WhenEditingAnotherOwnersProduct()
        {
            var id = await productService.CreateAsync(OtherOwnerId, "Theirs", null, 10m, 5, 1, 1, 1);

            var result = await productService.EditAsync(id, OwnerId, "Hacked", null, 1m, 1, 1, 1, 1);

            Assert.That(result, Is.False);

            var product = await data.Products.FindAsync(id);
            Assert.That(product!.Name, Is.EqualTo("Theirs"));
        }

        [Test]
        public async Task DeleteAsync_ShouldReturnFalse_WhenDeletingAnotherOwnersProduct()
        {
            var id = await productService.CreateAsync(OtherOwnerId, "Theirs", null, 10m, 5, 1, 1, 1);

            var result = await productService.DeleteAsync(id, OwnerId);

            Assert.That(result, Is.False);

            var product = await data.Products.FindAsync(id);
            Assert.That(product!.IsDeleted, Is.False);
        }
    }
}

using InventoryManagementSystem.Data;
using InventoryManagementSystem.Data.Models;
using InventoryManagementSystem.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace InventoryManagementSystem.Tests
{
    [TestFixture]
    public class StockTransactionServiceTests
    {
        private const string OwnerId = "owner-1";
        private const string OtherOwnerId = "owner-2";

        private InventoryDbContext data = null!;
        private StockTransactionService stockTransactionService = null!;
        private int testProductId;

        [SetUp]
        public async Task Setup()
        {
            var options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            data = new InventoryDbContext(options);

            data.Categories.Add(new Category { Id = 1, Name = "Electronics", OwnerId = OwnerId });
            data.Suppliers.Add(new Supplier { Id = 1, Name = "Supplier", Email = "s@s.com", PhoneNumber = "123", OwnerId = OwnerId });
            data.Warehouses.Add(new Warehouse { Id = 1, Name = "Warehouse", Location = "Location", OwnerId = OwnerId });

            var product = new Product
            {
                Name = "Test Product",
                Price = 10m,
                Quantity = 50,
                CategoryId = 1,
                SupplierId = 1,
                WarehouseId = 1,
                OwnerId = OwnerId
            };

            data.Products.Add(product);
            await data.SaveChangesAsync();

            testProductId = product.Id;
            stockTransactionService = new StockTransactionService(data);
        }

        [TearDown]
        public void TearDown()
        {
            data.Dispose();
        }

        [Test]
        public async Task AddStockAsync_ShouldIncreaseProductQuantity()
        {
            var result = await stockTransactionService.AddStockAsync(testProductId, 25, "Restock", OwnerId);

            Assert.That(result, Is.True);

            var product = await data.Products.FindAsync(testProductId);
            Assert.That(product!.Quantity, Is.EqualTo(75));
        }

        [Test]
        public async Task AddStockAsync_ShouldCreateTransaction()
        {
            await stockTransactionService.AddStockAsync(testProductId, 10, "Test note", OwnerId);

            var transactions = await data.StockTransactions.ToListAsync();
            Assert.That(transactions.Count, Is.EqualTo(1));
            Assert.That(transactions[0].QuantityChanged, Is.EqualTo(10));
            Assert.That(transactions[0].Type, Is.EqualTo("Add"));
            Assert.That(transactions[0].Note, Is.EqualTo("Test note"));
        }

        [Test]
        public async Task RemoveStockAsync_ShouldDecreaseProductQuantity()
        {
            var result = await stockTransactionService.RemoveStockAsync(testProductId, 20, "Sold", OwnerId);

            Assert.That(result, Is.True);

            var product = await data.Products.FindAsync(testProductId);
            Assert.That(product!.Quantity, Is.EqualTo(30));
        }

        [Test]
        public async Task RemoveStockAsync_ShouldReturnFalse_WhenInsufficientStock()
        {
            var result = await stockTransactionService.RemoveStockAsync(testProductId, 100, "Too much", OwnerId);

            Assert.That(result, Is.False);

            var product = await data.Products.FindAsync(testProductId);
            Assert.That(product!.Quantity, Is.EqualTo(50));
        }

        [Test]
        public async Task RemoveStockAsync_ShouldNotCreateTransaction_WhenInsufficientStock()
        {
            await stockTransactionService.RemoveStockAsync(testProductId, 100, "Too much", OwnerId);

            var transactions = await data.StockTransactions.ToListAsync();
            Assert.That(transactions.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task AddStockAsync_ShouldReturnFalse_WhenProductNotFound()
        {
            var result = await stockTransactionService.AddStockAsync(999, 10, "Note", OwnerId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task RemoveStockAsync_ShouldReturnFalse_WhenProductDeleted()
        {
            var product = await data.Products.FindAsync(testProductId);
            product!.IsDeleted = true;
            await data.SaveChangesAsync();

            var result = await stockTransactionService.RemoveStockAsync(testProductId, 10, "Note", OwnerId);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task AddStockAsync_ShouldReturnFalse_WhenProductBelongsToAnotherOwner()
        {
            var result = await stockTransactionService.AddStockAsync(testProductId, 10, "Note", OtherOwnerId);

            Assert.That(result, Is.False);

            var product = await data.Products.FindAsync(testProductId);
            Assert.That(product!.Quantity, Is.EqualTo(50));
        }
    }
}

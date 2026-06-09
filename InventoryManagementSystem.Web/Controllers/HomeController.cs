using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHomeService homeService;

        public HomeController(IHomeService homeService)
        {
            this.homeService = homeService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await homeService.GetDashboardDataAsync();

            var model = new DashboardViewModel
            {
                TotalProducts = data.TotalProducts,
                TotalCategories = data.TotalCategories,
                TotalSuppliers = data.TotalSuppliers,
                TotalWarehouses = data.TotalWarehouses,
                TotalStockQuantity = data.TotalStockQuantity,
                TotalInventoryValue = data.TotalInventoryValue,
                LowStockProducts = data.LowStockProducts,
                RecentTransactions = data.RecentTransactions
            };

            return View(model);
        }

        public async Task<IActionResult> Statistics()
        {
            var data = await homeService.GetDashboardDataAsync();
            var stats = await homeService.GetStatisticsAsync();

            var model = new StatisticsViewModel
            {
                TotalInventoryValue = data.TotalInventoryValue,
                TotalStockQuantity = data.TotalStockQuantity,
                TotalProducts = data.TotalProducts,
                TotalCategories = data.TotalCategories,
                TotalSuppliers = data.TotalSuppliers,
                TotalWarehouses = data.TotalWarehouses,
                LowStockCount = data.LowStockProducts.Count(),
                ByCategory = stats.ByCategory.Select(c => new CategoryBreakdown
                {
                    Name = c.Name,
                    ProductCount = c.ProductCount,
                    TotalValue = c.TotalValue,
                    TotalQuantity = c.TotalQuantity
                }),
                ByWarehouse = stats.ByWarehouse.Select(w => new WarehouseBreakdown
                {
                    Name = w.Name,
                    Location = w.Location,
                    ProductCount = w.ProductCount,
                    TotalValue = w.TotalValue,
                    TotalQuantity = w.TotalQuantity
                }),
                TopProductsByValue = stats.TopProductsByValue.Select(p => new TopProduct
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryName = p.CategoryName,
                    Price = p.Price,
                    Quantity = p.Quantity,
                    TotalValue = p.TotalValue
                })
            };

            return View(model);
        }
    }
}

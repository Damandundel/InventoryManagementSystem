using InventoryManagementSystem.Services.Models;

namespace InventoryManagementSystem.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalWarehouses { get; set; }
        public int TotalStockQuantity { get; set; }
        public decimal TotalInventoryValue { get; set; }
        public IEnumerable<ProductServiceModel> LowStockProducts { get; set; } = new List<ProductServiceModel>();
        public IEnumerable<StockTransactionServiceModel> RecentTransactions { get; set; } = new List<StockTransactionServiceModel>();
    }
}

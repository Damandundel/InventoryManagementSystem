namespace InventoryManagementSystem.Web.ViewModels
{
    public class StatisticsViewModel
    {
        public decimal TotalInventoryValue { get; set; }
        public int TotalStockQuantity { get; set; }
        public int TotalProducts { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalWarehouses { get; set; }
        public int LowStockCount { get; set; }

        public IEnumerable<CategoryBreakdown> ByCategory { get; set; } = new List<CategoryBreakdown>();
        public IEnumerable<WarehouseBreakdown> ByWarehouse { get; set; } = new List<WarehouseBreakdown>();
        public IEnumerable<TopProduct> TopProductsByValue { get; set; } = new List<TopProduct>();
    }

    public class CategoryBreakdown
    {
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public decimal TotalValue { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class WarehouseBreakdown
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public decimal TotalValue { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class TopProduct
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalValue { get; set; }
    }
}

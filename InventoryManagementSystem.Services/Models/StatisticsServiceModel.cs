namespace InventoryManagementSystem.Services.Models
{
    public class StatisticsServiceModel
    {
        public IEnumerable<CategoryBreakdownModel> ByCategory { get; set; } = new List<CategoryBreakdownModel>();
        public IEnumerable<WarehouseBreakdownModel> ByWarehouse { get; set; } = new List<WarehouseBreakdownModel>();
        public IEnumerable<TopProductModel> TopProductsByValue { get; set; } = new List<TopProductModel>();
    }

    public class CategoryBreakdownModel
    {
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public decimal TotalValue { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class WarehouseBreakdownModel
    {
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public decimal TotalValue { get; set; }
        public int TotalQuantity { get; set; }
    }

    public class TopProductModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalValue { get; set; }
    }
}

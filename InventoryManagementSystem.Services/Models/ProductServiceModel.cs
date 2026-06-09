namespace InventoryManagementSystem.Services.Models
{
    public class ProductServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
    }
}

namespace InventoryManagementSystem.Services.Models
{
    public class StockTransactionServiceModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantityChanged { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string? Note { get; set; }
    }
}

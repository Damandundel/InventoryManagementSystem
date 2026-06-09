namespace InventoryManagementSystem.Services.Models
{
    public class WarehouseServiceModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}

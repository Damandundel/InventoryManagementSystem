namespace InventoryManagementSystem.Web.ViewModels.Warehouse
{
    public class WarehouseDeleteViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}

namespace InventoryManagementSystem.Web.ViewModels.Supplier
{
    public class SupplierDeleteViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}

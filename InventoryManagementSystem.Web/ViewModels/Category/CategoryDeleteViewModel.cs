namespace InventoryManagementSystem.Web.ViewModels.Category
{
    public class CategoryDeleteViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }
}

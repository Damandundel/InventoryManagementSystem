using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Web.ViewModels.Product
{
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, 999999.99)]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        [Required]
        [Display(Name = "Warehouse")]
        public int WarehouseId { get; set; }

        public IEnumerable<CategoryDropdownViewModel> Categories { get; set; } = new List<CategoryDropdownViewModel>();
        public IEnumerable<SupplierDropdownViewModel> Suppliers { get; set; } = new List<SupplierDropdownViewModel>();
        public IEnumerable<WarehouseDropdownViewModel> Warehouses { get; set; } = new List<WarehouseDropdownViewModel>();
    }
}

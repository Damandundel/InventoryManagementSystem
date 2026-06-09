using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Web.ViewModels.StockTransaction
{
    public class StockTransactionFormViewModel
    {
        [Required]
        [Display(Name = "Product")]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Note { get; set; }

        public IEnumerable<ProductDropdownViewModel> Products { get; set; } = new List<ProductDropdownViewModel>();
    }
}

using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Web.ViewModels.Category
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}

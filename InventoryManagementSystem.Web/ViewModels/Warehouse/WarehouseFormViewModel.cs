using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystem.Web.ViewModels.Warehouse
{
    public class WarehouseFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Location { get; set; } = string.Empty;
    }
}

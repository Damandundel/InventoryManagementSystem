using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryManagementSystem.Data.Models
{
    public class StockTransaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; } = null!;

        [Required]
        public int QuantityChanged { get; set; }

        [Required]
        [MaxLength(10)]
        public string Type { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedOn { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}

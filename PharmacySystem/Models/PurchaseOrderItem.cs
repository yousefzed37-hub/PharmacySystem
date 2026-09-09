using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacySystem.Models
{
    public class PurchaseOrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(PurchaseOrder))]
        public int PurchaseOrderId { get; set; }

        [Required]
        [ForeignKey(nameof(Medicine))]
        public int MedicineId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public PurchaseOrder? PurchaseOrder { get; set; }
        public Medicine? Medicine { get; set; }
    }
}

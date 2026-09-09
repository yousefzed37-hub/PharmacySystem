using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacySystem.Models.DBModels
{
    public class PurchaseOrderItem
    {
        public int Id { get; set; }

        public int PurchaseOrderId { get; set; }

        public int MedicineId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public PurchaseOrder? PurchaseOrder { get; set; }
        public Medicine? Medicine { get; set; }
    }
}

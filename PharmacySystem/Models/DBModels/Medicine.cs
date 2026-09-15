    using PharmacySystem.Interface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.ServerSentEvents;

namespace PharmacySystem.Models.DBModels
{
    public class Medicine : ISoftDelete
    {
         public int Id { get; set; }


         public string Name { get; set; } = string.Empty;
         public decimal CostPrice { get; set; }

         public decimal SalePrice { get; set; }

         public int StockQuantity { get; set; }

         public int ReorderLevel { get; set; }

         public DateTime ExpiryDate { get; set; }
        // Additional properties for detailed information
        public string? ActiveIngredient { get; set; }
        public string? Indications { get; set; }
        public string? Dosage { get; set; }
        public string? SideEffects { get; set; }
        public string? Contraindications { get; set; }
        public string? ShelfLocation { get; set; }

        //----------------------------------------------

        public bool IsDeleted { get; set; } = false;

         public int CategoryId { get; set; }
         public Category? Category { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    }

}

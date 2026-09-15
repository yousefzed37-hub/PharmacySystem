    using PharmacySystem.Interface;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.ServerSentEvents;

namespace PharmacySystem.Models.DBModels
{
    public class Medicine : ISoftDelete
    {
         public int Id { get; set; }
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public decimal CostPrice { get; set; }

         public decimal SalePrice { get; set; }

         public int StockQuantity { get; set; }

         public int ReorderLevel { get; set; }

        public DateTime ExpiryDate { get; set; }
       
        
        public bool IsDeleted { get; set; } = false;

         public int CategoryId { get; set; }
         public Category? Category { get; set; }
        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
        public ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    }

}

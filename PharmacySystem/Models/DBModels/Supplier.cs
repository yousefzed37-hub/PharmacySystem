using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PharmacySystem.Models.DBModels
{
    public class Supplier
    {
       
        public int Id { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public string? ContactName { get; set; }

        public string Phone { get; set; } = string.Empty;

        public string? Address { get; set; }

        public string? ImageUrl { get; set; }

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
        
    }
}

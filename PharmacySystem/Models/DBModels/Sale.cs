using System.Net.ServerSentEvents;

namespace PharmacySystem.Models.DBModels
{
    public class Sale
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }

        public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    }
}

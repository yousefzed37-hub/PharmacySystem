using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.ViewModels
{
    public class SaleViewModel
    {
        [Display(Name = "Cashier / User ID")]
        public string? UserId { get; set; } = string.Empty;

        [Display(Name = "Discount")]
        [Range(0, double.MaxValue, ErrorMessage = "Discount cannot be negative.")]
        public decimal? Discount { get; set; } = 0;

        [Display(Name = "Sale Items")]
        [Required(ErrorMessage = "The sale must contain at least one item.")]
        [MinLength(1, ErrorMessage = "Please add at least one medicine to the invoice.")]
        public List<SaleItemViewModel> Items { get; set; } = new List<SaleItemViewModel>();
    }

    public class SaleDetailsViewModel
    {
        [Display(Name = "Sale ID")]
        public int Id { get; set; }

        [Display(Name = "Invoice Number")]
        public string InvoiceNumber { get; set; } = string.Empty;

        [Display(Name = "Cashier / User ID")]
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Sale Date")]
        public DateTime SaleDate { get; set; }

        [Display(Name = "Subtotal")]
        public decimal SubTotal { get; set; }

        [Display(Name = "Discount")]
        public decimal? Discount { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Items")]
        public List<SaleItemDetailsViewModel> Items { get; set; } = new List<SaleItemDetailsViewModel>();
    }
}

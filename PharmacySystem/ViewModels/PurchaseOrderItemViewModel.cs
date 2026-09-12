using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.ViewModels
{
    public class PurchaseOrderItemViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a medicine")]
        [Display(Name = "Medicine")]
        public int MedicineId { get; set; }

        public string? MedicineName { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Subtotal")]
        public decimal SubTotal => Quantity * UnitPrice;
    }
}

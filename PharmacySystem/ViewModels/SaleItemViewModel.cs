using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.ViewModels
{
    public class SaleItemViewModel    
    {
        [Display(Name = "Medicine")]
        [Required(ErrorMessage = "Please select a medicine.")]
        public int MedicineId { get; set; }

        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "Please specify the quantity.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Display(Name = "Unit Price")]
        [Required(ErrorMessage = "Please specify the unit price.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0.")]
        public decimal UnitPrice { get; set; }
    }

    public class SaleItemDetailsViewModel
    {
        [Display(Name = "Item ID")]
        public int Id { get; set; }

        [Display(Name = "Medicine ID")]
        public int MedicineId { get; set; }

        [Display(Name = "Medicine Name")]
        public string MedicineName { get; set; } = string.Empty;

        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Subtotal")]
        public decimal SubTotal { get; set; }
    }
}

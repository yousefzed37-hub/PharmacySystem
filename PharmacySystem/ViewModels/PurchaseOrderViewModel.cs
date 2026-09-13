using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.ViewModels
{
    public class PurchaseOrderViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a supplier")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        public string? SupplierName { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [MinLength(1, ErrorMessage = "Please add at least one medicine item to the order")]
        public List<PurchaseOrderItemViewModel> Items { get; set; } = new();

        [Display(Name = "Total Amount")]
        


        private decimal _totalCost;
        [Display(Name = "Total Amount")]
        public decimal TotalCost
        {
            get => (Items != null && Items.Any()) ? Items.Sum(i => i.Quantity * i.UnitPrice) : _totalCost;
            set => _totalCost = value;
        }

        public IEnumerable<SelectListItem>? SuppliersList { get; set; }
        public IEnumerable<SelectListItem>? MedicinesList { get; set; }
    }
}

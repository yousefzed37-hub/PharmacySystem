using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PharmacySystem.Models.ViewModels
{
    public class Purchase
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Please select a supplier")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [StringLength(500)]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
        [Required(ErrorMessage = "Please select a medicine")]
        [Display(Name = "Medicine")]
        public int MedicineId { get; set; }
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; } = 1;
        [Required(ErrorMessage = "Unit price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount => Quantity * UnitPrice;
        public IEnumerable<SelectListItem>? SuppliersList { get; set; }
        public IEnumerable<SelectListItem>? MedicinesList { get; set; }
    }
}

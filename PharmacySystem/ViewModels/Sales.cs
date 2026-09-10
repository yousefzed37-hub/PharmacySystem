using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PharmacySystem.Models.ViewModels
{
    public class Sales
    {
        public int Id { get; set; }

        // Sale Details
        [Required(ErrorMessage = "Sale date is required")]
        [Display(Name = "Sale Date")]
        public DateTime SaleDate { get; set; } = DateTime.Now;

        [Display(Name = "Customer Name")]
        [StringLength(100)]
        public string? CustomerName { get; set; }

        [Display(Name = "Notes")]
        [StringLength(500)]
        public string? Notes { get; set; }

        // Item Details
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

        [Display(Name = "Subtotal")]
        public decimal SubTotal => Quantity * UnitPrice;

        // Dropdown List for Medicines
        public IEnumerable<SelectListItem>? MedicinesList { get; set; }
    }
}
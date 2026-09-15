using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PharmacyManagement.Web.ViewModels.Medicines
{
    public class MedicineFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Trade name is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Trade name must be between 2 and 150 characters.")]
        [Display(Name = "Medicine Name")]
        public string Name { get; set; } = string.Empty;
 

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        
        public IEnumerable<SelectListItem>? Categories { get; set; }

 

        [Required(ErrorMessage = "Selling price is required.")]
        [Range(0.01, 100000.0, ErrorMessage = "Selling price must be greater than zero.")]
        [Display(Name = "Selling Price ($)")]
        public decimal SellingPrice { get; set; }

        [Required(ErrorMessage = "Cost price is required.")]
        [Range(0.01, 100000.0, ErrorMessage = "Cost price must be greater than zero.")]
        [Display(Name = "Cost Price ($)")]
        public decimal CostPrice { get; set; }

        [Range(0, 100000, ErrorMessage = "Stock quantity cannot be negative.")]
        [Display(Name = "Current Stock")]
        public int StockQuantity { get; set; } = 0;

        [Range(0, 500, ErrorMessage = "Reorder alert level cannot be negative.")]
        [Display(Name = "Reorder Alert Level")]
        public int ReorderLevel { get; set; } = 5;

        [DataType(DataType.Date)]
        [Display(Name = "Expiration Date")]
        public DateTime? ExpiryDate { get; set; }
        public string? ExistingImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public class MedicineIndexViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Medicine Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Category")]
        public string CategoryName { get; set; } = string.Empty;

        [Display(Name = "Image")]
        public string ImageUrl { get; set; } = string.Empty;
        public string? ExistingImageUrl { get; set; }

        [Display(Name = "Selling Price")]
        public decimal SellingPrice { get; set; }

        [Display(Name = "Stock")]
        public int StockQuantity { get; set; } 
     
        [Display(Name = "Expiry Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? ExpiryDate { get; set; }
        public int ReorderLevel { get; set; }
        public bool IsOutOfStock => StockQuantity <= 0;
        public bool IsLowStock => StockQuantity > 0 && StockQuantity <= ReorderLevel;
        public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow;
    }

    public class MedicineQuickCreateDto
    {
        public string Name { get; set; } = string.Empty;
        
        public int CategoryId { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int ReorderLevel { get; set; } = 5;
        public DateTime? ExpiryDate { get; set; }
    }
}
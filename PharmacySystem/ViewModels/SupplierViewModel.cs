using System.ComponentModel.DataAnnotations;

namespace PharmacySystem.ViewModels
{
    public class SupplierViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Company name must be between 3 and 150 characters.")]
        [Display(Name = "Company Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        [Display(Name = "Contact Number")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        [Display(Name = "Company Address")]
        public string? Address { get; set; }
    }
    public class SupplierIndexViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Supplier Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Phone")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Total Orders")]
        public int TotalPurchaseOrders { get; set; }
    }
}

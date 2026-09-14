using PharmacySystem.Models.DBModels;

namespace PharmacySystem.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalMedicines { get; set; }
        public int TotalCategories { get; set; }
        public int TotalSuppliers { get; set; }
        public int TotalInvoices { get; set; }

        public int ExpiringSoonCount { get; set; }
        public IEnumerable<Medicine> ExpiringMedicines { get; set; } = new List<Medicine>();
        public int LowStockCount { get; set; }
        public IEnumerable<Medicine> LowStockMedicines { get; set; } = new List<Medicine>();
    }
}
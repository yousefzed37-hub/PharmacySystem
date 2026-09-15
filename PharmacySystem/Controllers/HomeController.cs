using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Core.Interfaces;
using PharmacySystem.Models;
using PharmacySystem.ViewModels;
using System.Diagnostics;

namespace PharmacySystem.Controllers
{
    [Authorize(Roles ="Admin")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {

            var today = DateTime.UtcNow;
            var After30Day = today.AddDays(30);

            var expiringMedicines = await _unitOfWork.Medicines.FindAllAsync(
                criteria: m => m.ExpiryDate >= DateTime.UtcNow && m.ExpiryDate <= DateTime.UtcNow.AddDays(30),
                orderBy: m => m.ExpiryDate,
                orderByDirection: "ASC",
                take: 5
            );

            var lowStockMedicines = await _unitOfWork.Medicines.FindAllAsync(
                criteria: m => m.StockQuantity <= 5,
                orderBy: m => m.StockQuantity,
                orderByDirection: "ASC",
                take: 5
            );
            var viewModel = new DashboardViewModel
            {
                TotalMedicines = await _unitOfWork.Medicines.CountAsync(),
                TotalCategories = await _unitOfWork.Categories.CountAsync(),
                TotalSuppliers = await _unitOfWork.Suppliers.CountAsync(),
                TotalInvoices = await _unitOfWork.Sales.CountAsync(),
                ExpiringSoonCount = await _unitOfWork.Medicines.CountAsync(),
                ExpiringMedicines = expiringMedicines,
                LowStockCount = lowStockMedicines.Count(),
                LowStockMedicines = lowStockMedicines
            };
            return View(viewModel);
        }
    }
}

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
            var after30Days = today.AddDays(30);

            // 1. جلب الأدوية اللي هتنتهي قريباً (مع التأكيد الإضافي على تجاهل المحذوف)
            var expiringMedicines = await _unitOfWork.Medicines.FindAllAsync(
                criteria: m => m.IsDeleted == false && m.ExpiryDate >= today && m.ExpiryDate <= after30Days,
                orderBy: m => m.ExpiryDate,
                orderByDirection: "ASC",
                take: 5
            );

            // 2. جلب الأدوية النواقص (مع التأكيد الإضافي على تجاهل المحذوف)
            var lowStockMedicines = await _unitOfWork.Medicines.FindAllAsync(
                criteria: m => m.IsDeleted == false && m.StockQuantity <= 5,
                orderBy: m => m.StockQuantity,
                orderByDirection: "ASC",
                take: 5
            );

            // 3. تجهيز الـ ViewModel
            var viewModel = new DashboardViewModel
            {
                // دوال الـ CountAsync هنا هتعتمد بشكل كامل على الـ Global Query Filter 
                // اللي ضفناه في ملف الـ DbContext عشان تعد العناصر غير المحذوفة بس بشكل احترافي
                TotalMedicines = await _unitOfWork.Medicines.CountAsync(m=>m.IsDeleted ! ==false),
                TotalCategories = await _unitOfWork.Categories.CountAsync(m => m.IsDeleted! == false),
                TotalSuppliers = await _unitOfWork.Suppliers.CountAsync(),
                TotalInvoices = await _unitOfWork.Sales.CountAsync(),

                // حساب الأرقام دي مبني على القوائم المفلترة فوق، فمستحيل تعد حاجة محذوفة
                ExpiringSoonCount = expiringMedicines.Count(),
                ExpiringMedicines = expiringMedicines,

                LowStockCount = lowStockMedicines.Count(),
                LowStockMedicines = lowStockMedicines
            };

            return View(viewModel);
        }
    }
}

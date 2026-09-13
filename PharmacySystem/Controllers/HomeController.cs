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
            var viewModel = new DashboardViewModel
            {
                TotalMedicines = await _unitOfWork.Medicines.CountAsync(),
                TotalCategories = await _unitOfWork.Categories.CountAsync(),
                TotalSuppliers = await _unitOfWork.Suppliers.CountAsync(),
                TotalInvoices = await _unitOfWork.Sales.CountAsync(),
            };
            return View(viewModel);
        }
    }
}

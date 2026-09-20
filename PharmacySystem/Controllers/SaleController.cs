using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PharmacyManagement.Core.Interfaces;
using PharmacySystem.Models.DBModels;
using PharmacySystem.ViewModels;
using System.Security.Claims;

namespace PharmacySystem.Controllers
{
    [Authorize(Roles = "Admin , Pharmacist , Cashier")]
    public class SaleController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public SaleController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm)
        {
            var sales = await _unitOfWork.Sales.FindAllAsync(
                criteria: s => true,
                includes: new[] { "SaleItems", "SaleItems.Medicine" }
            );

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var cleanSearch = searchTerm.Trim();
                sales = sales.Where(s => s.InvoiceNumber.Contains(cleanSearch, StringComparison.OrdinalIgnoreCase));
            }

            var salesListViewModel = sales.Select(sale => new SaleDetailsViewModel
            {
                Id = sale.Id,
                InvoiceNumber = sale.InvoiceNumber,
                SaleDate = sale.SaleDate,
                SubTotal = sale.SubTotal,
                Discount = sale.Discount,
                TotalAmount = sale.TotalAmount,
                Items = sale.SaleItems.Select(item => new SaleItemDetailsViewModel
                {
                    Id = item.Id,
                    MedicineId = item.MedicineId,
                    MedicineName = item.Medicine?.Name ?? string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.SubTotal
                }).ToList()
            }).OrderByDescending(s => s.SaleDate).ToList();

            return View(salesListViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var medicines = await _unitOfWork.Medicines.FindAllAsync(m => m.StockQuantity > 0);
            ViewBag.Medicines = new SelectList(medicines, "Id", "Name");

            var model = new SaleViewModel
            {
                Items = new List<SaleItemViewModel> { new SaleItemViewModel() }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SaleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Medicines = await _unitOfWork.Medicines.FindAllAsync(m => m.StockQuantity > 0);
                return View(model);
            }

            var sale = new Sale
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMddHHmmss}",
                SaleDate = DateTime.UtcNow,
                Discount = model.Discount
            };

            decimal subTotal = 0;

            foreach (var item in model.Items)
            {
                var medicine = await _unitOfWork.Medicines.GetByIdAsync(item.MedicineId);

                if (medicine == null || medicine.StockQuantity < item.Quantity)
                {
                    ModelState.AddModelError("", $"The requested quantity for {(medicine?.Name ?? "selected medicine")} is not available in stock.");
                    var medicinesList = await _unitOfWork.Medicines.FindAllAsync(m => m.StockQuantity > 0);
                    ViewBag.Medicines = new SelectList(medicinesList, "Id", "Name");
                    return View(model);
                }

                medicine.StockQuantity -= item.Quantity;
                _unitOfWork.Medicines.Update(medicine);

                decimal itemSubTotal = item.Quantity * item.UnitPrice;
                subTotal += itemSubTotal;

                sale.SaleItems.Add(new SaleItem
                {
                    MedicineId = item.MedicineId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = itemSubTotal
                });
            }

            sale.SubTotal = subTotal;
            sale.TotalAmount = subTotal - (model.Discount ?? 0);

            await _unitOfWork.Sales.AddAsync(sale);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Details), new { id = sale.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var sale = await _unitOfWork.Sales.FindAsync(
                criteria: s => s.Id == id,
                includes: new[] { "SaleItems", "SaleItems.Medicine" }
            );

            if (sale == null)
            {
                return NotFound();
            }

            var detailsViewModel = new SaleDetailsViewModel
            {
                Id = sale.Id,
                InvoiceNumber = sale.InvoiceNumber,
                SaleDate = sale.SaleDate,
                SubTotal = sale.SubTotal,
                Discount = sale.Discount,
                TotalAmount = sale.TotalAmount,
                Items = sale.SaleItems.Select(item => new SaleItemDetailsViewModel
                {
                    Id = item.Id,
                    MedicineId = item.MedicineId,
                    MedicineName = item.Medicine?.Name ?? string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.SubTotal
                }).ToList()
            };

            return View(detailsViewModel);
        }
    }
}
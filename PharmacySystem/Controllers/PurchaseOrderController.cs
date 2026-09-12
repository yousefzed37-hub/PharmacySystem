using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PharmacyManagement.Core.Interfaces;
using PharmacySystem.Core.Constants;
using PharmacySystem.Models.DBModels;
using PharmacySystem.ViewModels;

namespace PharmacySystem.Controllers
{
    //[Authorize(Roles = $"{AppConstants.Roles.Admin},{AppConstants.Roles.Pharmacist}")]
    public class PurchaseOrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseOrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /PurchaseOrders
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _unitOfWork.PurchaseOrders.GetAllAsync();
            var suppliers = (await _unitOfWork.Suppliers.GetAllAsync()).ToDictionary(s => s.Id, s => s.CompanyName);

            var modelList = orders.Select(o => new PurchaseOrderViewModel
            {
                Id = o.Id,
                SupplierId = o.SupplierId,
                SupplierName = suppliers.ContainsKey(o.SupplierId) ? suppliers[o.SupplierId] : "N/A",
                OrderDate = o.OrderDate
            }).OrderByDescending(o => o.OrderDate).ToList();

            return View(modelList);
        }

        // GET: /PurchaseOrders/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new PurchaseOrderViewModel
            {
                OrderDate = DateTime.Now,
                SuppliersList = await GetSuppliersSelectListAsync()
            };

            return View(model);
        }

        // POST: /PurchaseOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SuppliersList = await GetSuppliersSelectListAsync();
                return View(model);
            }

            var order = new PurchaseOrder
            {
                SupplierId = model.SupplierId,
                OrderDate = model.OrderDate,
                TotalCost = 0 // Initial amount before items are added
            };

            await _unitOfWork.PurchaseOrders.AddAsync(order);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Purchase order created. You can now add medicine items.";
            return RedirectToAction("Create", "PurchaseOrderItems", new { orderId = order.Id });
        }

        // GET: /PurchaseOrders/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _unitOfWork.PurchaseOrders.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(order.SupplierId);

            var model = new PurchaseOrderViewModel
            {
                Id = order.Id,
                SupplierId = order.SupplierId,
                SupplierName = supplier?.CompanyName ?? "N/A",
                OrderDate = order.OrderDate
            };

            return View(model);
        }

        private async Task<IEnumerable<SelectListItem>> GetSuppliersSelectListAsync()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            return suppliers.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.CompanyName
            });
        }
    }
}

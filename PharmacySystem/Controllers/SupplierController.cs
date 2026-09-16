using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Core.Interfaces;
using PharmacySystem.Core.Constants;
using PharmacySystem.Models.DBModels;
using PharmacySystem.ViewModels;

namespace PharmacySystem.Controllers
{
    [Authorize(Roles = "Admin , Pharmacist")]
    //[Authorize(Roles = $"{AppConstants.Roles.Admin},{AppConstants.Roles.Pharmacist}")]
    public class SupplierController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SupplierController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }

        // GET: /Suppliers
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            var orders = await _unitOfWork.PurchaseOrders.GetAllAsync();

            var viewModel = suppliers.Select(s => new SupplierIndexViewModel
            {
                Id = s.Id,
                Name = s.CompanyName,
                Phone = s.Phone,
                Email = s.ContactName,
                TotalPurchaseOrders = orders.Count(o => o.SupplierId == s.Id)
            }).ToList();

            return View(viewModel);
        }

        // GET: /Suppliers/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new SupplierViewModel());
        }

        // POST: /Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var supplier = new Supplier
            {
                CompanyName = model.Name,
                ContactName = model.Email ?? string.Empty,
                Phone = model.Phone,
                Address = model.Address ?? string.Empty,
            };

            await _unitOfWork.Suppliers.AddAsync(supplier);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Supplier added successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Suppliers/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (supplier == null)
                return NotFound();

            var model = new SupplierViewModel
            {
                Id = supplier.Id,
                Name = supplier.CompanyName,
                Phone = supplier.Phone,
                Email = supplier.ContactName,
                Address = supplier.Address
            };

            return View(model);
        }

        // POST: /Suppliers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SupplierViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(model.Id);
            if (supplier == null)
                return NotFound();

            supplier.CompanyName = model.Name;
            supplier.ContactName = model.Email ?? string.Empty;
            supplier.Phone = model.Phone;
            supplier.Address = model.Address ?? string.Empty;

            _unitOfWork.Suppliers.Update(supplier);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = "Supplier updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        //// POST: /Suppliers/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = AppConstants.Roles.Admin)]
        //public async Task<IActionResult> Delete(int id)
        //{
        //    var supplier = await _unitOfWork.Suppliers.GetByIdAsync(id);
        //    if (supplier == null)
        //        return Json(new { success = false, message = "Supplier not found." });

        //    var relatedOrders = await _unitOfWork.PurchaseOrders.FindAllAsync(po => po.SupplierId == id);
        //    if (relatedOrders.Any())
        //    {
        //        return Json(new { success = false, message = "Cannot delete supplier with existing purchase orders." });
        //    }


        //    _unitOfWork.Suppliers.Delete(supplier);
        //    await _unitOfWork.CompleteAsync();

        //    return Json(new { success = true, message = "Supplier deleted successfully." });
        //}

    }
}

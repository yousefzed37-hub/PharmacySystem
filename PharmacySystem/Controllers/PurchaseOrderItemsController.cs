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
    public class PurchaseOrderItemsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PurchaseOrderItemsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET: /PurchaseOrderItems/Index?orderId=5
        [HttpGet]
        public async Task<IActionResult> Index(int orderId)
        {
            ViewBag.OrderId = orderId;
            var items = await _unitOfWork.PurchaseOrderItems.FindAllAsync(i => i.PurchaseOrderId == orderId);
            var medicines = (await _unitOfWork.Medicines.GetAllAsync()).ToDictionary(m => m.Id, m => m.Name);

            var modelList = items.Select(i => new PurchaseOrderItemViewModel
            {
                Id = i.Id,
                MedicineId = i.MedicineId,
                MedicineName = medicines.ContainsKey(i.MedicineId) ? medicines[i.MedicineId] : "N/A",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            return View(modelList);
        }

        // GET: /PurchaseOrderItems/Create?orderId=5
        [HttpGet]
        public async Task<IActionResult> Create(int orderId)
        {
            var order = await _unitOfWork.PurchaseOrders.GetByIdAsync(orderId);
            if (order == null)
                return NotFound("Purchase order not found.");

            ViewBag.OrderId = orderId;

            var model = new PurchaseOrderItemViewModel
            {
                Quantity = 1
            };

            await LoadMedicinesListAsync();
            return View(model);
        }

        // POST: /PurchaseOrderItems/Create?orderId=5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int orderId, PurchaseOrderItemViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.OrderId = orderId;
                await LoadMedicinesListAsync();
                return View(model);
            }

            var order = await _unitOfWork.PurchaseOrders.GetByIdAsync(orderId);
            if (order == null)
                return NotFound("Purchase order not found.");

            var medicine = await _unitOfWork.Medicines.GetByIdAsync(model.MedicineId);
            if (medicine == null)
                return NotFound("Selected medicine does not exist.");

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. Add line item
                var orderItem = new PurchaseOrderItem
                {
                    PurchaseOrderId = orderId,
                    MedicineId = model.MedicineId,
                    Quantity = model.Quantity,
                    UnitPrice = model.UnitPrice
                };
                await _unitOfWork.PurchaseOrderItems.AddAsync(orderItem);

                // 2. Increment medicine stock and update cost price
                medicine.StockQuantity += model.Quantity;
                medicine.CostPrice = model.UnitPrice;
                _unitOfWork.Medicines.Update(medicine);

                // 3. Update order total cost
                order.TotalCost += (model.Quantity * model.UnitPrice);
                _unitOfWork.PurchaseOrders.Update(order);

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Item added and inventory updated successfully.";
                return RedirectToAction(nameof(Index), new { orderId = orderId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"Error saving item: {ex.Message}");
                ViewBag.OrderId = orderId;
                await LoadMedicinesListAsync();
                return View(model);
            }
        }

        // POST: /PurchaseOrderItems/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.PurchaseOrderItems.GetByIdAsync(id);
            if (item == null)
                return NotFound();

            var order = await _unitOfWork.PurchaseOrders.GetByIdAsync(item.PurchaseOrderId);
            var medicine = await _unitOfWork.Medicines.GetByIdAsync(item.MedicineId);

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // Revert received quantity from stock
                if (medicine != null)
                {
                    if (medicine.StockQuantity < item.Quantity)
                    {
                        TempData["Error"] = "Cannot delete item: part of this stock has already been sold.";
                        return RedirectToAction(nameof(Index), new { orderId = item.PurchaseOrderId });
                    }

                    medicine.StockQuantity -= item.Quantity;
                    _unitOfWork.Medicines.Update(medicine);
                }

                // Deduct item total from parent purchase order
                if (order != null)
                {
                    order.TotalCost -= (item.Quantity * item.UnitPrice);
                    _unitOfWork.PurchaseOrders.Update(order);
                }

                _unitOfWork.PurchaseOrderItems.Delete(item);

                await _unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                TempData["Success"] = "Item deleted and inventory adjusted successfully.";
                return RedirectToAction(nameof(Index), new { orderId = item.PurchaseOrderId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = $"Error deleting item: {ex.Message}";
                return RedirectToAction(nameof(Index), new { orderId = item.PurchaseOrderId });
            }
        }

        private async Task LoadMedicinesListAsync()
        {
            var medicines = await _unitOfWork.Medicines.GetAllAsync();
            ViewBag.MedicinesList = medicines.Where(m => !m.IsDeleted).Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.Name} (Current Stock: {m.StockQuantity})"
            });
        }
    }
}

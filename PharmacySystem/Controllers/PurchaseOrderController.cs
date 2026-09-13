using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PharmacyManagement.Core.Interfaces;
using PharmacySystem.Core.Constants;
using PharmacySystem.Models.DBModels;
using PharmacySystem.ViewModels;

namespace PharmacySystem.Controllers
{
    [Authorize(Roles = "Admin , Pharmacist")]
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

            var viewModels = orders.Select(o => new PurchaseOrderViewModel
            {
                Id = o.Id,
                SupplierId = o.SupplierId,
                SupplierName = suppliers.ContainsKey(o.SupplierId) ? suppliers[o.SupplierId] : "N/A",
                OrderDate = o.OrderDate,
                TotalCost = o.TotalCost,
                Notes = string.Empty // رأس الجدول في الـ Index
            }).OrderByDescending(o => o.OrderDate).ToList();

            return View(viewModels);
        }

        // GET: /PurchaseOrders/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _unitOfWork.PurchaseOrders.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            var supplier = await _unitOfWork.Suppliers.GetByIdAsync(order.SupplierId);
            var items = await _unitOfWork.PurchaseOrderItems.FindAllAsync(poi => poi.PurchaseOrderId == id);
            var medicines = (await _unitOfWork.Medicines.GetAllAsync()).ToDictionary(m => m.Id, m => m.Name);

            var viewModel = new PurchaseOrderViewModel
            {
                Id = order.Id,
                SupplierId = order.SupplierId,
                SupplierName = supplier?.CompanyName ?? "N/A",
                OrderDate = order.OrderDate,
                Items = items.Select(i => new PurchaseOrderItemViewModel
                {
                    Id = i.Id,
                    MedicineId = i.MedicineId,
                    MedicineName = medicines.ContainsKey(i.MedicineId) ? medicines[i.MedicineId] : "Unknown Medicine",
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: /PurchaseOrders/Create
        // GET: /PurchaseOrder/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new PurchaseOrderViewModel
            {
                OrderDate = DateTime.Now,
                SuppliersList = await GetSuppliersSelectListAsync(),
                MedicinesList = await GetMedicinesSelectListAsync()
            };

            // ده السطر اللي كان ناقص ومخلي الأقسام مش باينة نهائياً:
            ViewBag.CategoriesList = (await _unitOfWork.Categories.GetAllAsync())
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();

            return View(model);
        }

        // POST: /PurchaseOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrderViewModel model)
        {
            if (model.Items == null || !model.Items.Any())
            {
                ModelState.AddModelError(string.Empty, "Please add at least one medicine item to the order.");
            }

            if (!ModelState.IsValid)
            {
                model.SuppliersList = await GetSuppliersSelectListAsync();
                model.MedicinesList = await GetMedicinesSelectListAsync();

                
                ViewBag.CategoriesList = (await _unitOfWork.Categories.GetAllAsync())
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });

                return View(model);
            }

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. إنشاء وحفظ رأس الفاتورة
                var purchaseOrder = new PurchaseOrder
                {
                    SupplierId = model.SupplierId,
                    OrderDate = model.OrderDate,
                    TotalCost = model.TotalCost // محسوبة تلقائياً في الـ ViewModel
                };

                await _unitOfWork.PurchaseOrders.AddAsync(purchaseOrder);
                await _unitOfWork.CompleteAsync(); // لتوليد الـ Id الخاص بأمر الشراء

                // 2. معالجة بنود الفاتورة وتحديث المخزون وسعر التكلفة
                var orderItems = new List<PurchaseOrderItem>();

                foreach (var item in model.Items)
                {
                    var medicine = await _unitOfWork.Medicines.GetByIdAsync(item.MedicineId);
                    if (medicine == null)
                        throw new InvalidOperationException($"Medicine with ID {item.MedicineId} does not exist.");

                    // زيادة رصيد المخزن وتحديث سعر الشراء
                    medicine.StockQuantity += item.Quantity;
                    medicine.CostPrice = item.UnitPrice;
                    _unitOfWork.Medicines.Update(medicine);

                    // إضافة سطر الطلبية
                    orderItems.Add(new PurchaseOrderItem
                    {
                        PurchaseOrderId = purchaseOrder.Id,
                        MedicineId = item.MedicineId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    });
                }

                // حفظ جميع البنود دفعة واحدة
                await _unitOfWork.PurchaseOrderItems.AddRangeAsync(orderItems);
                await _unitOfWork.CompleteAsync();

                // تثبيت الـ Transaction
                await transaction.CommitAsync();

                TempData["Success"] = $"Purchase Order #{purchaseOrder.Id} created and stock updated successfully.";
                return RedirectToAction(nameof(Details), new { id = purchaseOrder.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ModelState.AddModelError(string.Empty, $"Error processing order: {ex.Message}");
                model.SuppliersList = await GetSuppliersSelectListAsync();
                model.MedicinesList = await GetMedicinesSelectListAsync();
                return View(model);
            }
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

        private async Task<IEnumerable<SelectListItem>> GetMedicinesSelectListAsync()
        {
            var medicines = await _unitOfWork.Medicines.GetAllAsync();
            return medicines.Where(m => !m.IsDeleted).Select(m => new SelectListItem
            {
                Value = m.Id.ToString(),
                Text = $"{m.Name} (Stock: {m.StockQuantity})"
            });
        }
    }
}

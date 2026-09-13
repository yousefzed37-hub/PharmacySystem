using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PharmacyManagement.Core.Interfaces;
using PharmacyManagement.Web.ViewModels.Medicines;
using PharmacySystem.Models.DBModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PharmacyManagement.Web.Controllers
{
    [Authorize(Roles = "Admin , Pharmacist")]
    public class MedicineController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public MedicineController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var medicines = await _unitOfWork.Medicines.FindAllAsync(m => !m.IsDeleted, new[] { "Category" });

            var viewModels = medicines.Select(m => new MedicineIndexViewModel
            {
                Id = m.Id,
                Name = m.Name,
                SellingPrice = m.SalePrice,
                StockQuantity = m.StockQuantity,
                ExpiryDate = m.ExpiryDate,
                CategoryName = m.Category?.Name ?? "Uncategorized",
                IsLowStock = m.StockQuantity <= m.ReorderLevel
            }).ToList();

            return View(viewModels);
        }

        public async Task<IActionResult> Create()
        {
            var categories = await _unitOfWork.Categories.GetAllAsync();

            var viewModel = new MedicineFormViewModel
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicineFormViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                viewModel.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                return View(viewModel);
            }

            var newMedicine = new Medicine
            {
                Name = viewModel.Name,
                SalePrice = viewModel.SellingPrice,
                CostPrice = viewModel.CostPrice,
                CategoryId = viewModel.CategoryId,
                StockQuantity = viewModel.StockQuantity,
                ReorderLevel = viewModel.ReorderLevel,
                ExpiryDate = viewModel.ExpiryDate ?? DateTime.MaxValue
            };

            await _unitOfWork.Medicines.AddAsync(newMedicine);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await _unitOfWork.Medicines.GetByIdAsync(id);

            if (medicine == null) return NotFound();

            var categories = await _unitOfWork.Categories.GetAllAsync();

            var viewModel = new MedicineFormViewModel
            {
                Id = medicine.Id,
                Name = medicine.Name,
                SellingPrice = medicine.SalePrice,
                CostPrice = medicine.CostPrice,
                CategoryId = medicine.CategoryId,
                StockQuantity = medicine.StockQuantity,
                ReorderLevel = medicine.ReorderLevel,
                ExpiryDate = medicine.ExpiryDate,
                Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MedicineFormViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                var categories = await _unitOfWork.Categories.GetAllAsync();
                viewModel.Categories = categories.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name });
                return View(viewModel);
            }

            var medicine = await _unitOfWork.Medicines.GetByIdAsync(id);
            if (medicine == null) return NotFound();

            medicine.Name = viewModel.Name;
            medicine.SalePrice = viewModel.SellingPrice;
            medicine.CostPrice = viewModel.CostPrice;
            medicine.CategoryId = viewModel.CategoryId;
            medicine.StockQuantity = viewModel.StockQuantity;
            medicine.ReorderLevel = viewModel.ReorderLevel;
            medicine.ExpiryDate = viewModel.ExpiryDate ?? medicine.ExpiryDate;

            _unitOfWork.Medicines.Update(medicine);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var medicine = (await _unitOfWork.Medicines.FindAllAsync(m => m.Id == id, new[] { "Category" })).FirstOrDefault();

            if (medicine == null) return NotFound();

            var viewModel = new MedicineIndexViewModel
            {
                Id = medicine.Id,
                Name = medicine.Name,
                SellingPrice = medicine.SalePrice,
                CategoryName = medicine.Category?.Name ?? "Uncategorized"
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicine = await _unitOfWork.Medicines.GetByIdAsync(id);
            if (medicine == null) return NotFound();

            medicine.IsDeleted = true;
            _unitOfWork.Medicines.Update(medicine);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }



        [HttpPost]
        public async Task<IActionResult> QuickCreate([FromBody] MedicineQuickCreateDto dto)
        {
            try
            {
                if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || dto.CategoryId <= 0)
                {
                    return Json(new { success = false, message = "Invalid medicine data or missing category." });
                }

                // فحص بدون ToLower عشان نتفادى مشاكل ترجمة SQL
                var cleanName = dto.Name.Trim();
                var existing = await _unitOfWork.Medicines.FindAsync(m => m.Name == cleanName && !m.IsDeleted);
                if (existing != null)
                {
                    return Json(new { success = false, message = "A medicine with this name already exists!" });
                }

                var medicine = new Medicine
                {
                    Name = cleanName,
                    CategoryId = dto.CategoryId,
                    CostPrice = dto.CostPrice,
                    SalePrice = dto.SalePrice,
                    StockQuantity = 0,
                    ReorderLevel = dto.ReorderLevel > 0 ? dto.ReorderLevel : 5,
                    ExpiryDate = dto.ExpiryDate ?? DateTime.UtcNow.AddYears(2),
                    IsDeleted = false
                };

                await _unitOfWork.Medicines.AddAsync(medicine);
                await _unitOfWork.CompleteAsync();

                return Json(new
                {
                    success = true,
                    id = medicine.Id,
                    name = medicine.Name,
                    costPrice = medicine.CostPrice
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "DB Error: " + ex.Message });
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using PharmacyManagement.Core.Interfaces;
using PharmacyManagement.Web.ViewModels.Categories;
using PharmacySystem.Models;
using PharmacySystem.Models.DBModels;
using System.Linq;
using System.Threading.Tasks;

namespace PharmacyManagement.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _unitOfWork.Categories.FindAllAsync(c => !c.IsDeleted);

            var viewModels = categories.Select(c => new CategoryIndexViewModel
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return View(viewModels);
        }

        public IActionResult Create()
        {
            var viewModel = new CategoryFormViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            var newCategory = new Category
            {
                Name = viewModel.Name
            };

            await _unitOfWork.Categories.AddAsync(newCategory);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null) return NotFound();

            var viewModel = new CategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel viewModel)
        {
            if (id != viewModel.Id) return BadRequest();

            if (!ModelState.IsValid) return View(viewModel);

            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return NotFound();

            category.Name = viewModel.Name;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null) return NotFound();

            var viewModel = new CategoryIndexViewModel
            {
                Id = category.Id,
                Name = category.Name
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return NotFound();

            category.IsDeleted = true;

            _unitOfWork.Categories.Update(category);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
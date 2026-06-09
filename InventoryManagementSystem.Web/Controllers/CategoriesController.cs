using InventoryManagementSystem.Data.Constants;
using InventoryManagementSystem.Services.Constants;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Web.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await categoryService.GetAllAsync();
            return View(categories);
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        public IActionResult Create()
        {
            return View(new CategoryFormViewModel());
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await categoryService.CreateAsync(model.Name, model.Description);

            TempData["Success"] = string.Format(MessageConstants.SuccessCreate, "Category");

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await categoryService.GetByIdAsync(id);

            if (category == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "category");
                return RedirectToAction(nameof(Index));
            }

            var model = new CategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await categoryService.EditAsync(id, model.Name, model.Description);

            if (!result)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "category");
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = string.Format(MessageConstants.SuccessEdit, "Category");

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await categoryService.GetByIdAsync(id);

            if (category == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "category");
                return RedirectToAction(nameof(Index));
            }

            var model = new CategoryDeleteViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ProductCount = category.ProductCount
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await categoryService.DeleteAsync(id);

            if (!result)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "category");
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = string.Format(MessageConstants.SuccessDelete, "Category");

            return RedirectToAction(nameof(Index));
        }
    }
}

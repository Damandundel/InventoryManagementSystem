using InventoryManagementSystem.Services.Constants;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Web.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly ICategoryService categoryService;
        private readonly IProductService productService;

        public CategoriesController(
            ICategoryService categoryService,
            IProductService productService)
        {
            this.categoryService = categoryService;
            this.productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await categoryService.GetAllAsync(OwnerId);
            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await categoryService.GetByIdAsync(id, OwnerId);

            if (category == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "category");
                return RedirectToAction(nameof(Index));
            }

            var products = await productService.GetByCategoryAsync(id, OwnerId);

            var model = new CategoryDetailsViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ProductCount = category.ProductCount,
                TotalQuantity = products.Sum(p => p.Quantity),
                TotalValue = products.Sum(p => p.Price * p.Quantity),
                Products = products
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new CategoryFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await categoryService.CreateAsync(OwnerId, model.Name, model.Description);

            TempData["Success"] = string.Format(MessageConstants.SuccessCreate, "Category");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await categoryService.GetByIdAsync(id, OwnerId);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await categoryService.EditAsync(id, OwnerId, model.Name, model.Description);

            if (!result)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "category");
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = string.Format(MessageConstants.SuccessEdit, "Category");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await categoryService.GetByIdAsync(id, OwnerId);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await categoryService.DeleteAsync(id, OwnerId);

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

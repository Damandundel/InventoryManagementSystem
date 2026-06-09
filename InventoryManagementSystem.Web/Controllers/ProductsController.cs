using InventoryManagementSystem.Data.Constants;
using InventoryManagementSystem.Services.Constants;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Web.ViewModels;
using InventoryManagementSystem.Web.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly ISupplierService supplierService;
        private readonly IWarehouseService warehouseService;

        public ProductsController(
            IProductService productService,
            ICategoryService categoryService,
            ISupplierService supplierService,
            IWarehouseService warehouseService)
        {
            this.productService = productService;
            this.categoryService = categoryService;
            this.supplierService = supplierService;
            this.warehouseService = warehouseService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await productService.GetAllAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await productService.GetByIdAsync(id);

            if (product == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "product");
                return RedirectToAction(nameof(Index));
            }

            var model = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                CategoryName = product.CategoryName,
                SupplierName = product.SupplierName,
                WarehouseName = product.WarehouseName
            };

            return View(model);
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        public async Task<IActionResult> Create()
        {
            var model = new ProductFormViewModel();
            await PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            await productService.CreateAsync(
                model.Name, model.Description, model.Price,
                model.Quantity, model.CategoryId, model.SupplierId, model.WarehouseId);

            TempData["Success"] = string.Format(MessageConstants.SuccessCreate, "Product");

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await productService.GetByIdAsync(id);

            if (product == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "product");
                return RedirectToAction(nameof(Index));
            }

            var model = new ProductFormViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId,
                WarehouseId = product.WarehouseId
            };

            await PopulateDropdowns(model);

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            var result = await productService.EditAsync(
                id, model.Name, model.Description, model.Price,
                model.Quantity, model.CategoryId, model.SupplierId, model.WarehouseId);

            if (!result)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "product");
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = string.Format(MessageConstants.SuccessEdit, "Product");

            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await productService.GetByIdAsync(id);

            if (product == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "product");
                return RedirectToAction(nameof(Index));
            }

            var model = new ProductDeleteViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryName = product.CategoryName
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await productService.DeleteAsync(id);

            if (!result)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "product");
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = string.Format(MessageConstants.SuccessDelete, "Product");

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdowns(ProductFormViewModel model)
        {
            var categories = await categoryService.GetAllAsync();
            var suppliers = await supplierService.GetAllAsync();
            var warehouses = await warehouseService.GetAllAsync();

            model.Categories = categories.Select(c => new CategoryDropdownViewModel { Id = c.Id, Name = c.Name });
            model.Suppliers = suppliers.Select(s => new SupplierDropdownViewModel { Id = s.Id, Name = s.Name });
            model.Warehouses = warehouses.Select(w => new WarehouseDropdownViewModel { Id = w.Id, Name = w.Name });
        }
    }
}

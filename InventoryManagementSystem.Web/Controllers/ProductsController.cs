using InventoryManagementSystem.Services.Constants;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Web.ViewModels;
using InventoryManagementSystem.Web.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IProductService productService;
        private readonly ICategoryService categoryService;
        private readonly ISupplierService supplierService;
        private readonly IWarehouseService warehouseService;
        private readonly IStockTransactionService stockTransactionService;

        public ProductsController(
            IProductService productService,
            ICategoryService categoryService,
            ISupplierService supplierService,
            IWarehouseService warehouseService,
            IStockTransactionService stockTransactionService)
        {
            this.productService = productService;
            this.categoryService = categoryService;
            this.supplierService = supplierService;
            this.warehouseService = warehouseService;
            this.stockTransactionService = stockTransactionService;
        }

        public async Task<IActionResult> Index()
        {
            var products = await productService.GetAllAsync(OwnerId);
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await productService.GetByIdAsync(id, OwnerId);

            if (product == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "product");
                return RedirectToAction(nameof(Index));
            }

            var transactions = await stockTransactionService.GetByProductIdAsync(id, OwnerId);

            var model = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Quantity = product.Quantity,
                CategoryId = product.CategoryId,
                CategoryName = product.CategoryName,
                SupplierId = product.SupplierId,
                SupplierName = product.SupplierName,
                WarehouseId = product.WarehouseId,
                WarehouseName = product.WarehouseName,
                Transactions = transactions
            };

            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            var model = new ProductFormViewModel();
            await PopulateDropdowns(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            await productService.CreateAsync(
                OwnerId, model.Name, model.Description, model.Price,
                model.Quantity, model.CategoryId, model.SupplierId, model.WarehouseId);

            TempData["Success"] = string.Format(MessageConstants.SuccessCreate, "Product");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await productService.GetByIdAsync(id, OwnerId);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdowns(model);
                return View(model);
            }

            var result = await productService.EditAsync(
                id, OwnerId, model.Name, model.Description, model.Price,
                model.Quantity, model.CategoryId, model.SupplierId, model.WarehouseId);

            if (!result)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "product");
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = string.Format(MessageConstants.SuccessEdit, "Product");

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await productService.GetByIdAsync(id, OwnerId);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await productService.DeleteAsync(id, OwnerId);

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
            var categories = await categoryService.GetAllAsync(OwnerId);
            var suppliers = await supplierService.GetAllAsync(OwnerId);
            var warehouses = await warehouseService.GetAllAsync(OwnerId);

            model.Categories = categories.Select(c => new CategoryDropdownViewModel { Id = c.Id, Name = c.Name });
            model.Suppliers = suppliers.Select(s => new SupplierDropdownViewModel { Id = s.Id, Name = s.Name });
            model.Warehouses = warehouses.Select(w => new WarehouseDropdownViewModel { Id = w.Id, Name = w.Name });
        }
    }
}

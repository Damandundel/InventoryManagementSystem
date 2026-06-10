using InventoryManagementSystem.Services.Constants;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Web.ViewModels.Warehouse;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers
{
    public class WarehousesController : BaseController
    {
        private readonly IWarehouseService warehouseService;
        private readonly IProductService productService;

        public WarehousesController(
            IWarehouseService warehouseService,
            IProductService productService)
        {
            this.warehouseService = warehouseService;
            this.productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var warehouses = await warehouseService.GetAllAsync(OwnerId);
            return View(warehouses);
        }

        public async Task<IActionResult> Details(int id)
        {
            var warehouse = await warehouseService.GetByIdAsync(id, OwnerId);

            if (warehouse == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "warehouse");
                return RedirectToAction(nameof(Index));
            }

            var products = await productService.GetByWarehouseAsync(id, OwnerId);

            var model = new WarehouseDetailsViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location,
                ProductCount = warehouse.ProductCount,
                TotalQuantity = products.Sum(p => p.Quantity),
                TotalValue = products.Sum(p => p.Price * p.Quantity),
                Products = products
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new WarehouseFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await warehouseService.CreateAsync(OwnerId, model.Name, model.Location);

            TempData["Success"] = string.Format(MessageConstants.SuccessCreate, "Warehouse");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var warehouse = await warehouseService.GetByIdAsync(id, OwnerId);

            if (warehouse == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "warehouse");
                return RedirectToAction(nameof(Index));
            }

            var model = new WarehouseFormViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WarehouseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await warehouseService.EditAsync(id, OwnerId, model.Name, model.Location);

            if (!result)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "warehouse");
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = string.Format(MessageConstants.SuccessEdit, "Warehouse");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var warehouse = await warehouseService.GetByIdAsync(id, OwnerId);

            if (warehouse == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "warehouse");
                return RedirectToAction(nameof(Index));
            }

            var model = new WarehouseDeleteViewModel
            {
                Id = warehouse.Id,
                Name = warehouse.Name,
                Location = warehouse.Location,
                ProductCount = warehouse.ProductCount
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await warehouseService.DeleteAsync(id, OwnerId);

            if (!result)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "warehouse");
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = string.Format(MessageConstants.SuccessDelete, "Warehouse");

            return RedirectToAction(nameof(Index));
        }
    }
}

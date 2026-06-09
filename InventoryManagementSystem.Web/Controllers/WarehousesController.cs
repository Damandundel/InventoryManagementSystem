using InventoryManagementSystem.Data.Constants;
using InventoryManagementSystem.Services.Constants;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Web.ViewModels.Warehouse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers
{
    public class WarehousesController : Controller
    {
        private readonly IWarehouseService warehouseService;

        public WarehousesController(IWarehouseService warehouseService)
        {
            this.warehouseService = warehouseService;
        }

        public async Task<IActionResult> Index()
        {
            var warehouses = await warehouseService.GetAllAsync();
            return View(warehouses);
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        public IActionResult Create()
        {
            return View(new WarehouseFormViewModel());
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WarehouseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await warehouseService.CreateAsync(model.Name, model.Location);

            TempData["Success"] = string.Format(MessageConstants.SuccessCreate, "Warehouse");

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        public async Task<IActionResult> Edit(int id)
        {
            var warehouse = await warehouseService.GetByIdAsync(id);

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
        [Authorize(Roles = RoleConstants.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WarehouseFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await warehouseService.EditAsync(id, model.Name, model.Location);

            if (!result)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "warehouse");
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = string.Format(MessageConstants.SuccessEdit, "Warehouse");

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        public async Task<IActionResult> Delete(int id)
        {
            var warehouse = await warehouseService.GetByIdAsync(id);

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
        [Authorize(Roles = RoleConstants.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await warehouseService.DeleteAsync(id);

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

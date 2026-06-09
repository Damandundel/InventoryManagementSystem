using InventoryManagementSystem.Data.Constants;
using InventoryManagementSystem.Services.Constants;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Web.ViewModels.Supplier;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly ISupplierService supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            this.supplierService = supplierService;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await supplierService.GetAllAsync();
            return View(suppliers);
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        public IActionResult Create()
        {
            return View(new SupplierFormViewModel());
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await supplierService.CreateAsync(model.Name, model.Email, model.PhoneNumber);

            TempData["Success"] = string.Format(MessageConstants.SuccessCreate, "Supplier");

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await supplierService.GetByIdAsync(id);

            if (supplier == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "supplier");
                return RedirectToAction(nameof(Index));
            }

            var model = new SupplierFormViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Email = supplier.Email,
                PhoneNumber = supplier.PhoneNumber
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await supplierService.EditAsync(id, model.Name, model.Email, model.PhoneNumber);

            if (!result)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "supplier");
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = string.Format(MessageConstants.SuccessEdit, "Supplier");

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = RoleConstants.Administrator)]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await supplierService.GetByIdAsync(id);

            if (supplier == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "supplier");
                return RedirectToAction(nameof(Index));
            }

            var model = new SupplierDeleteViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Email = supplier.Email,
                ProductCount = supplier.ProductCount
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = RoleConstants.Administrator)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await supplierService.DeleteAsync(id);

            if (!result)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "supplier");
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = string.Format(MessageConstants.SuccessDelete, "Supplier");

            return RedirectToAction(nameof(Index));
        }
    }
}

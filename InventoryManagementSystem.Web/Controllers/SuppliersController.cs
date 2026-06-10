using InventoryManagementSystem.Services.Constants;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Web.ViewModels.Supplier;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers
{
    public class SuppliersController : BaseController
    {
        private readonly ISupplierService supplierService;
        private readonly IProductService productService;

        public SuppliersController(
            ISupplierService supplierService,
            IProductService productService)
        {
            this.supplierService = supplierService;
            this.productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await supplierService.GetAllAsync(OwnerId);
            return View(suppliers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var supplier = await supplierService.GetByIdAsync(id, OwnerId);

            if (supplier == null)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "supplier");
                return RedirectToAction(nameof(Index));
            }

            var products = await productService.GetBySupplierAsync(id, OwnerId);

            var model = new SupplierDetailsViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                Email = supplier.Email,
                PhoneNumber = supplier.PhoneNumber,
                ProductCount = supplier.ProductCount,
                TotalQuantity = products.Sum(p => p.Quantity),
                TotalValue = products.Sum(p => p.Price * p.Quantity),
                Products = products
            };

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new SupplierFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await supplierService.CreateAsync(OwnerId, model.Name, model.Email, model.PhoneNumber);

            TempData["Success"] = string.Format(MessageConstants.SuccessCreate, "Supplier");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await supplierService.GetByIdAsync(id, OwnerId);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await supplierService.EditAsync(id, OwnerId, model.Name, model.Email, model.PhoneNumber);

            if (!result)
            {
                TempData["Error"] = string.Format(MessageConstants.ErrorNotFound, "supplier");
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = string.Format(MessageConstants.SuccessEdit, "Supplier");

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await supplierService.GetByIdAsync(id, OwnerId);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await supplierService.DeleteAsync(id, OwnerId);

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

using InventoryManagementSystem.Data.Constants;
using InventoryManagementSystem.Services.Constants;
using InventoryManagementSystem.Services.Contracts;
using InventoryManagementSystem.Web.ViewModels;
using InventoryManagementSystem.Web.ViewModels.StockTransaction;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers
{
    public class StockTransactionsController : BaseController
    {
        private readonly IStockTransactionService stockTransactionService;
        private readonly IProductService productService;

        public StockTransactionsController(
            IStockTransactionService stockTransactionService,
            IProductService productService)
        {
            this.stockTransactionService = stockTransactionService;
            this.productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var transactions = await stockTransactionService.GetAllAsync(OwnerId);
            return View(transactions);
        }

        public async Task<IActionResult> Create(int? productId)
        {
            var model = new StockTransactionFormViewModel
            {
                Type = StockTransactionTypes.Add
            };

            if (productId.HasValue && await productService.ExistsAsync(productId.Value, OwnerId))
            {
                model.ProductId = productId.Value;
            }

            await PopulateProducts(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockTransactionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateProducts(model);
                return View(model);
            }

            bool result;

            if (model.Type == StockTransactionTypes.Add)
            {
                result = await stockTransactionService.AddStockAsync(model.ProductId, model.Quantity, model.Note, OwnerId);
            }
            else
            {
                result = await stockTransactionService.RemoveStockAsync(model.ProductId, model.Quantity, model.Note, OwnerId);
            }

            if (!result)
            {
                TempData["Error"] = model.Type == StockTransactionTypes.Remove
                    ? MessageConstants.ErrorInsufficientStock
                    : MessageConstants.ErrorGeneral;
                await PopulateProducts(model);
                return View(model);
            }

            TempData["Success"] = model.Type == StockTransactionTypes.Add
                ? MessageConstants.SuccessStockAdd
                : MessageConstants.SuccessStockRemove;

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateProducts(StockTransactionFormViewModel model)
        {
            var products = await productService.GetAllAsync(OwnerId);
            model.Products = products.Select(p => new ProductDropdownViewModel { Id = p.Id, Name = p.Name });
        }
    }
}

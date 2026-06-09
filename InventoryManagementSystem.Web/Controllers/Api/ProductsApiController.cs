using InventoryManagementSystem.Services.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsApiController : ControllerBase
    {
        private readonly IProductService productService;

        public ProductsApiController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                var all = await productService.GetAllAsync();
                return Ok(all);
            }

            var products = await productService.SearchAsync(query);
            return Ok(products);
        }
    }
}

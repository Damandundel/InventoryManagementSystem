using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/400")]
        public IActionResult Error400()
        {
            Response.StatusCode = 400;
            return View();
        }

        [Route("Error/401")]
        public IActionResult Error401()
        {
            Response.StatusCode = 401;
            return View();
        }

        [Route("Error/404")]
        public IActionResult Error404()
        {
            Response.StatusCode = 404;
            return View();
        }

        [Route("Error/500")]
        public IActionResult Error500()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}

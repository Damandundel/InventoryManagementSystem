using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Web.Controllers
{
    /// <summary>
    /// Base for all controllers that operate on the signed-in user's own data.
    /// Requires authentication and exposes the current user's id as the owner key.
    /// </summary>
    [Authorize]
    public abstract class BaseController : Controller
    {
        protected string OwnerId => User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Authenticated user has no identifier.");
    }
}

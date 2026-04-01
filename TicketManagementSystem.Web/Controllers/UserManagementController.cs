using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagementSystem.Common.Constants;

namespace TicketManagementSystem.Web.Controllers
{
    [Authorize(Roles =RoleConstants.Admin)]
    public class UserManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

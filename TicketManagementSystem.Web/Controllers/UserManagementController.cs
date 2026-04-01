using Microsoft.AspNetCore.Mvc;

namespace TicketManagementSystem.Web.Controllers
{
    public class UserManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

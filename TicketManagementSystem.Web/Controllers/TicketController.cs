using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TicketManagementSystem.Web.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

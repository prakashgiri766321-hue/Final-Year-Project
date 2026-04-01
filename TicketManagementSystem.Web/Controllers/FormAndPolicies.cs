using Microsoft.AspNetCore.Mvc;

namespace TicketManagementSystem.Web.Controllers
{
    public class FormAndPolicies : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

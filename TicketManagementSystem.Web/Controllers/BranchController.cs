using Microsoft.AspNetCore.Mvc;
using TicketManagementSystem.Domain.Entities;
using TicketManagementSystem.Infrastructure.Data;

namespace TicketManagementSystem.Web.Controllers
{
    public class BranchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BranchController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var branches = _context.Branches.ToList();
            return View(branches);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Branch branch)
        {
            if (ModelState.IsValid)
            {
                _context.Branches.Add(branch);
                _context.SaveChanges();

                TempData["Success"] = "Branch created successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(branch);
        }
    }
}
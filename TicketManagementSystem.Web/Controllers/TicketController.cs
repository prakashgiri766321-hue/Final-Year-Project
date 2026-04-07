using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TicketManagementSystem.Application.DTOs;
using TicketManagementSystem.Infrastructure.Data;

namespace TicketManagementSystem.Web.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ApplicationDbContext _context;

        public TicketController(ITicketService ticketService, ApplicationDbContext context)
        {
            _ticketService = ticketService;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateTicketDto dto)
        {
            if (ModelState.IsValid)
            {
                _ticketService.CreateTicket(dto);
                return RedirectToAction("Index");
            }

            LoadDropdowns();
            return View(dto);
        }

        private void LoadDropdowns()
        {
            ViewBag.Branches = new SelectList(_context.Branches, "BranchId", "BranchCode");
        }
    }
}
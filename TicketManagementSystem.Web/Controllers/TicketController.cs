using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketManagementSystem.Application.DTOs;
using TicketManagementSystem.Common.Enum;
using TicketManagementSystem.Infrastructure.Data;
using TicketManagementSystem.Infrastructure.Identity;

namespace TicketManagementSystem.Web.Controllers
{
    [Authorize]
    public class TicketController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketController(
            ITicketService ticketService,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _ticketService = ticketService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(
            TicketStatus? status = null,
            int? branchId = null,
            bool myTickets = false,
            string? search = null,
            int page = 1)
        {
            var user = await _userManager.GetUserAsync(User);

            var filter = new TicketFilterDto
            {
                Status = status,
                BranchId = branchId,
                MyTickets = myTickets,
                Search = search,
                PageNumber = page,
                PageSize = 10
            };

            var result = _ticketService.GetTicketsPaged(filter, user?.Id);

            ViewBag.Users = await _context.Users.AsNoTracking().ToListAsync();
            ViewBag.TotalCount = result.TotalCount;
            ViewBag.PageNumber = result.PageNumber;
            ViewBag.PageSize = result.PageSize;
            ViewBag.TotalPages = result.TotalPages;
            ViewBag.FilterStatus = status;
            ViewBag.FilterBranchId = branchId;
            ViewBag.FilterMyTickets = myTickets;
            ViewBag.FilterSearch = search ?? string.Empty;
            ViewBag.AllBranches = await _context.Branches.AsNoTracking().OrderBy(b => b.BranchCode).ToListAsync();

            return View(result.Items.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTicketDto dto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Challenge();

                dto.CreatedById = user.Id;

                _ticketService.CreateTicket(dto);

                return RedirectToAction("Index");
            }

            LoadDropdowns();
            return View(dto);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var ticket = _ticketService.GetTicketById(id);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        [HttpPost]
        public async Task<IActionResult> Assign(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            _ticketService.AssignTicket(id, user.Id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Resolve(int id)
        {
            _ticketService.ResolveTicket(id);
            return RedirectToAction("Index");
        }

        private void LoadDropdowns()
        {
            ViewBag.Branches = new SelectList(_context.Branches, "BranchId", "BranchCode");
        }
    }
}
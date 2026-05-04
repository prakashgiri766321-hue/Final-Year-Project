using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TicketManagementSystem.Application.DTOs;
using TicketManagementSystem.Common.Constants;
using TicketManagementSystem.Infrastructure.Identity;

namespace TicketManagementSystem.Web.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            var roleMap = new Dictionary<string, string>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                roleMap[user.Id] = roles.FirstOrDefault() ?? "-";
            }

            ViewBag.UserRoles = roleMap;
            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            LoadRoleDropdown();
            return View(new CreateUserDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                LoadRoleDropdown();
                return View(dto);
            }

            if (!await _roleManager.RoleExistsAsync(dto.Role))
            {
                ModelState.AddModelError(nameof(dto.Role), "Selected role does not exist.");
                LoadRoleDropdown();
                return View(dto);
            }

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                UserName = dto.UserName,
                Email = dto.Email
            };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                LoadRoleDropdown();
                return View(dto);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, dto.Role);
            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await _userManager.DeleteAsync(user);
                LoadRoleDropdown();
                return View(dto);
            }

            TempData["SuccessMessage"] = $"User '{dto.UserName}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        private void LoadRoleDropdown()
        {
            var roles = _roleManager.Roles
                .OrderBy(r => r.Name)
                .Select(r => r.Name)
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .ToList();

            ViewBag.Roles = new SelectList(roles);
        }
    }
}

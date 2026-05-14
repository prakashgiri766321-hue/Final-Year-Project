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

            var filteredUsers = new List<ApplicationUser>();
            var roleMap = new Dictionary<string, string>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "-";
                if (role == RoleConstants.Admin)
                    continue;

                filteredUsers.Add(user);
                roleMap[user.Id] = role;
            }

            ViewBag.UserRoles = roleMap;
            return View(filteredUsers);
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
                            .Where(r =>
                                !string.IsNullOrWhiteSpace(r) &&
                                r != RoleConstants.Admin)
                            .ToList();

            ViewBag.Roles = new SelectList(roles);
        }
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Role = roles.FirstOrDefault() ?? "-";

            return View(user);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var dto = new EditUserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                Role = roles.FirstOrDefault() ?? ""
            };

            LoadRoleDropdown();

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserDto dto)
        {
            if (!ModelState.IsValid)
            {
                LoadRoleDropdown();
                return View(dto);
            }

            var user = await _userManager.FindByIdAsync(dto.Id);

            if (user == null)
                return NotFound();

            user.FullName = dto.FullName;
            user.UserName = dto.UserName;
            user.Email = dto.Email;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                LoadRoleDropdown();
                return View(dto);
            }

            var existingRoles = await _userManager.GetRolesAsync(user);

            await _userManager.RemoveFromRolesAsync(user, existingRoles);

            await _userManager.AddToRoleAsync(user, dto.Role);

            TempData["SuccessMessage"] = "User updated successfully.";

            return RedirectToAction(nameof(Index));
        }


    }
}

using eDrive3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eDrive3.Controllers
{
    [Authorize(Roles = "Admin")]  // Only admins can access
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Admin/CreateUser
        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: /Admin/CreateUser
        [HttpPost]
        public async Task<IActionResult> CreateUser(string email, string password, string role)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                ModelState.AddModelError("", "Role does not exist.");
                return View();
            }

            var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                return RedirectToAction("Index", "Home"); // Or wherever you want to redirect
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }
        }
    }
}

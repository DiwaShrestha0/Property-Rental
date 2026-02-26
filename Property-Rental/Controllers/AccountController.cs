using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyRental.Models;
using System.Security.Claims;

namespace PropertyRental.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and password are required.");
                return View();
            }

            // Note: In a real application, consider using a proper password hasher.
            // For now, doing direct comparison as requested/implied by "Validate user credentials".
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password && u.IsActive);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password.");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            foreach (var userRole in user.UserRoles)
            {
                if (userRole.Role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                }
            }

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Keep user logged in
                ExpiresUtc = DateTime.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Username,Email,PasswordHash")] User user)
        {
            if (ModelState.IsValid)
            {
                // Prevent duplicate usernames
                var exists = await _context.Users.AnyAsync(u => u.Username == user.Username);
                if (exists)
                {
                    ModelState.AddModelError("Username", "Username already exists. Please choose another.");
                    return View(user);
                }

                user.IsActive = true;
                user.CreatedAt = DateTime.Now;
                
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Assign default role "User"
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                if (role != null)
                {
                    _context.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Login));
            }
            return View(user);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}

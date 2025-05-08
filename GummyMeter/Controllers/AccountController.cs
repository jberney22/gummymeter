using GummyMeter.Data;
using GummyMeter.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GummyMeter.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                TempData["LoginFailed"] = true;
                ModelState.AddModelError(string.Empty, "Username and Password are required.");
                return RedirectToAction("Index", "Movies");
            }
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username.ToLower());
            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                TempData["LoginFailed"] = true;
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return RedirectToAction("Index", "Movies");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Movies");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return PartialView("_RegisterForm");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Return validation errors
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { key = x.Key, errors = x.Value.Errors.Select(e => e.ErrorMessage) });
                return Json(new { success = false, errors });
            }
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == vm.Username.ToLower()))
                {
                    ModelState.AddModelError(nameof(vm.Username), "Username already taken.");
                    var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                                           .Select(x => new { key = x.Key, errors = x.Value.Errors.Select(e => e.ErrorMessage) });
                    return Json(new { success = false, errors });
                }

                using var hmac = new HMACSHA512();
                var user = new User
                {
                    Username = vm.Username.ToLower(),
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(vm.Password)),
                    PasswordSalt = hmac.Key,
                    Email = vm.Email
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username)
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity));

            }
            catch (Exception)
            {
                var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                                   .Select(x => new { key = x.Key, errors = x.Value.Errors.Select(e => e.ErrorMessage) });
                return Json(new { success = false, errors });
            }


            return Json(new { success = true, message = "success" });
        }


        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Success"] = "Logged out successfully!";
            return RedirectToAction("Index", "Movies");
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            using var hmac = new HMACSHA512(storedSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(storedHash);
        }
    }
}

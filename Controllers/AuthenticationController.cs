using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClotherS.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly DataContext _context;

        public AuthenticationController(DataContext context)
        {
            _context = context;
        }

        // GET: Authentication/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Authentication/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);

            if (account != null && BCrypt.Net.BCrypt.Verify(password, account.Password))
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
                    new Claim(ClaimTypes.Name, account.Email),
                    new Claim("FullName", account.FirstName + " " + account.LastName),
                    new Claim(ClaimTypes.Role, account.RoleId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }

        // GET: Authentication/Register
        public IActionResult Register()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Authentication/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("AccountId,Email,FirstName,LastName,Phone,Password,AccountImage,Address,Gender,Active,Description,RoleId,DateOfBirth,Disable")] Account account)
        {
            var existingAccount = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == account.Email);
            if (existingAccount != null)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
                return View(account);
            }
            if (ModelState.IsValid)
            {
                account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // GET: Authentication/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}

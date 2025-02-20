using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace ClotherS.Controllers
{
    public class AccountsController : Controller
    {
        private readonly DataContext _context;

        public AccountsController(DataContext context)
        {
            _context = context;
        }

        // GET: Accounts
        [Authorize(Roles = "1")]
        public async Task<IActionResult> Index()
        {
            var dataContext = _context.Accounts.Include(a => a.Role);
            return View(await dataContext.ToListAsync());
        }

        // GET: Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Accounts/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            return View();
        }

        // POST: Accounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,Email,FirstName,LastName,Phone,Password,AccountImage,Address,Gender,Active,Description,RoleId,DateOfBirth,Disable")] Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // GET: Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // POST: Accounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,Email,FirstName,LastName,Phone,Password,AccountImage,Address,Gender,Active,Description,RoleId,DateOfBirth,Disable")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewBag.RoleId = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
                return View(account);
            }

            try
            {
                _context.Update(account);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(account.AccountId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Accounts/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Accounts/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email && a.Password == password);
            if (account != null)
            {
                var claims = new List<Claim>
        {
                    new Claim(ClaimTypes.NameIdentifier, account.AccountId.ToString()),
            new Claim(ClaimTypes.Name, account.Email),
            new Claim("FullName", account.FirstName + " " + account.LastName),
            new Claim(ClaimTypes.Role, account.RoleId.ToString()) // Lưu RoleId dưới dạng chuỗi
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid email or password";
            return View();
        }


        // GET: Accounts/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: Accounts/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Account account)
        {
            if (ModelState.IsValid)
            {
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(account);
        }

        // GET: Accounts/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }
    
    // GET: Accounts/Profile
public async Task<IActionResult> Profile()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var email = User.Identity.Name;
            var account = await _context.Accounts.Include(a => a.Role).FirstOrDefaultAsync(a => a.Email == email);

            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Accounts/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([Bind("AccountId,Email,FirstName,LastName,Phone,AccountImage,Address,Gender,Description,DateOfBirth")] Account account)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login");
            }

            var existingAccount = await _context.Accounts.FindAsync(account.AccountId);
            if (existingAccount == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                existingAccount.FirstName = account.FirstName;
                existingAccount.LastName = account.LastName;
                existingAccount.Phone = account.Phone;
                existingAccount.Address = account.Address;
                existingAccount.Gender = account.Gender;
                existingAccount.Description = account.Description;
                existingAccount.DateOfBirth = account.DateOfBirth;

                try
                {
                    _context.Update(existingAccount);
                    await _context.SaveChangesAsync();
                    ViewBag.Success = "Profile updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(existingAccount);
        }
    }
    }

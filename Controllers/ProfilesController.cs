using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ClotherS.Models;
using ClotherS.Repositories;

[Authorize]
public class ProfilesController : Controller
{
    private readonly DataContext _context;

    public ProfilesController(DataContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (!User.Identity.IsAuthenticated)
        {
            Console.WriteLine("User is not authenticated. Redirecting to login.");
            return RedirectToAction("Login", "Accounts");
        }

        var email = User.Identity.Name;
        Console.WriteLine($"Fetching profile for email: {email}");

        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);

        if (account == null)
        {
            Console.WriteLine("Error: Account not found.");
            return NotFound();
        }

        return View(account);
    }

    [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Index(int AccountId)
{
    if (!User.Identity.IsAuthenticated)
    {
        return RedirectToAction("Login", "Accounts");
    }

    var existingAccount = await _context.Accounts.FindAsync(AccountId);
    if (existingAccount == null)
    {
        return NotFound();
    }

    if (await TryUpdateModelAsync(
        existingAccount,
        "",
        a => a.FirstName, a => a.LastName, a => a.Phone, 
        a => a.Address, a => a.Gender, a => a.Description, 
        a => a.DateOfBirth))
    {
        try
        {
            await _context.SaveChangesAsync();
            ViewBag.Success = "Profile updated successfully!";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Accounts.Any(e => e.AccountId == AccountId))
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

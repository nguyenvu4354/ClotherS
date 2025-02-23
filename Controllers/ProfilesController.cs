using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            return RedirectToAction("Login", "Accounts");
        }

        var email = User.Identity.Name;
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);

        if (account == null)
        {
            return NotFound();
        }

        return View(account);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile([Bind("AccountId,FirstName,LastName,Phone,Address,Gender,Description,DateOfBirth")] Account model)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Accounts");
        }

        var existingAccount = await _context.Accounts.FindAsync(model.AccountId);
        if (existingAccount == null)
        {
            return NotFound();
        }

        existingAccount.FirstName = model.FirstName;
        existingAccount.LastName = model.LastName;
        existingAccount.Phone = model.Phone;
        existingAccount.Address = model.Address;
        existingAccount.Gender = model.Gender;
        existingAccount.Description = model.Description;
        existingAccount.DateOfBirth = model.DateOfBirth;

        try
        {
            await _context.SaveChangesAsync();
            ViewBag.Success = "Profile updated successfully!";
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Accounts.Any(e => e.AccountId == model.AccountId))
            {
                return NotFound();
            }
            throw;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfileImage(int AccountId, IFormFile profileImage)
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

        if (profileImage != null && profileImage.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }
            existingAccount.AccountImage = fileName;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Orders()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Accounts");
        }

        var email = User.Identity.Name;
        var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Email == email);

        if (account == null)
        {
            return NotFound();
        }

        var orders = await _context.OrderDetails
    .Where(od => od.Order.AccountId == account.AccountId && !od.Order.IsCart)
    .Include(od => od.Order)
    .Include(od => od.Product)
    .OrderByDescending(od => od.OrderDate) 
    .ToListAsync();


        return View(orders);
    }

    public async Task<IActionResult> OrderDetails(int id)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Accounts");
        }

        var orderDetails = await _context.OrderDetails
            .Where(od => od.OId == id)
            .Include(od => od.Order)
            .Include(od => od.Product)
            .ToListAsync();

        if (orderDetails == null || !orderDetails.Any())
        {
            return NotFound();
        }

        return View(orderDetails);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Accounts");
        }

        var order = await _context.Orders
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o => o.OId == orderId);

        if (order == null)
        {
            return NotFound();
        }

        // Chỉ cho phép hủy nếu trạng thái hiện tại là "Processing"
        if (order.OrderDetails.Any(od => od.Status != "Processing"))
        {
            TempData["Error"] = "Đơn hàng không thể hủy vì không ở trạng thái Processing!";
            return RedirectToAction("OrderDetails", new { id = orderId });
        }

        // Cập nhật trạng thái của tất cả OrderDetails trong đơn hàng
        foreach (var detail in order.OrderDetails)
        {
            detail.Status = "Disabled";
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Đơn hàng đã bị hủy thành công!";
        return RedirectToAction("OrderDetails", new { id = orderId });
    }




}

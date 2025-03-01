using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClotherS.Models;
using System.Security.Claims;
using ClotherS.Repositories;

[Authorize]
public class ProfilesController : Controller
{
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;
    private readonly DataContext _context;

    public ProfilesController(UserManager<Account> userManager, SignInManager<Account> signInManager, DataContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Accounts");
        }
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfile([Bind("Id,FirstName,LastName,PhoneNumber,Address,Gender,Description,DateOfBirth")] Account model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Accounts");
        }

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.PhoneNumber = model.PhoneNumber;
        user.Address = model.Address;
        user.Gender = model.Gender;
        user.Description = model.Description;
        user.DateOfBirth = model.DateOfBirth;

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            ViewBag.Success = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View("Index", user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateProfileImage(IFormFile profileImage)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Accounts");
        }

        if (profileImage != null && profileImage.Length > 0)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(profileImage.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            user.AccountImage = fileName;
            await _userManager.UpdateAsync(user);
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Orders()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Accounts");
        }

        var orders = await _context.OrderDetails
            .Where(od => od.Order.AccountId == user.Id && !od.Order.IsCart)
            .Include(od => od.Order)
            .Include(od => od.Product)
            .OrderByDescending(od => od.OrderDate)
            .ToListAsync();

        return View(orders);
    }

    public async Task<IActionResult> OrderDetails(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Accounts");
        }

        var orderDetails = await _context.OrderDetails
            .Include(od => od.Product)
            .Include(od => od.Feedbacks)
            .Where(od => od.OId == id && od.Order.AccountId == user.Id)
            .ToListAsync();

        if (!orderDetails.Any())
        {
            TempData["Error"] = "Không tìm thấy đơn hàng!";
            return RedirectToAction("Orders");
        }

        return View(orderDetails);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelOrder(int detailId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Accounts");
        }

        var orderDetail = await _context.OrderDetails
            .Include(od => od.Order)
            .FirstOrDefaultAsync(od => od.DetailId == detailId && od.Order.AccountId == user.Id);

        if (orderDetail == null)
        {
            return NotFound();
        }

        if (orderDetail.Status != "Processing")
        {
            TempData["Error"] = "Chỉ có thể hủy đơn hàng khi đang ở trạng thái Processing!";
            return RedirectToAction("OrderDetails", new { id = orderDetail.OId });
        }

        orderDetail.Status = "Disabled";
        var allDetails = await _context.OrderDetails.Where(od => od.OId == orderDetail.OId).ToListAsync();
        if (allDetails.All(od => od.Status == "Disabled"))
        {
            orderDetail.Order.IsCart = false;
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = "Đơn hàng đã bị hủy thành công!";
        return RedirectToAction("OrderDetails", new { id = orderDetail.OId });
    }
}

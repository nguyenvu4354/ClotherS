using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class FeedbacksController : Controller
{
    private readonly DataContext _context;

    public FeedbacksController(DataContext context)
    {
        _context = context;
    }

    // GET: Feedbacks/Create
    public async Task<IActionResult> Create(int orderDetailId)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Accounts");
        }

        var orderDetail = await _context.OrderDetails
            .Include(od => od.Order)
            .FirstOrDefaultAsync(od => od.OId == orderDetailId);

        if (orderDetail == null || orderDetail.Order.AccountId != GetCurrentUserId())
        {
            return NotFound();
        }

        var feedback = new Feedback
        {
            DetailId = orderDetailId,
            ProductId = orderDetail.ProductId,
            AccountId = GetCurrentUserId()
        };

        return View(feedback);
    }

    // POST: Feedbacks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Content,Rating,DetailId,ProductId,AccountId")] Feedback feedback)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login", "Accounts");
        }

        if (ModelState.IsValid)
        {
            feedback.CreatedAt = DateTime.Now;

            _context.Add(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction("Orders", "Profiles"); 
        }

        return View(feedback);
    }




    // Helper method to get the current user's ID
    private int GetCurrentUserId()
    {
        var email = User.Identity.Name;
        var account = _context.Accounts.FirstOrDefault(a => a.Email == email);
        return account?.AccountId ?? 0;
    }



}

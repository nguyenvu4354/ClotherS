using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace ClotherS.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly DataContext _context;

        public FeedbacksController(DataContext context)
        {
            _context = context;
        }

        // Hiển thị form đánh giá
        public IActionResult Create(int detailId)
        {
            var orderDetail = _context.OrderDetails
                .FirstOrDefault(od => od.DetailId == detailId);

            if (orderDetail == null || orderDetail.IsReviewed)
            {
                TempData["Error"] = "Không thể đánh giá đơn hàng này.";
                return RedirectToAction("OrderDetails", "Profiles");
            }

            var feedback = new Feedback { DetailId = detailId, ProductId = orderDetail.ProductId };
            return View(feedback);
        }

        [HttpPost]
        public IActionResult Create(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                return View(feedback);
            }

            var orderDetail = _context.OrderDetails.FirstOrDefault(od => od.DetailId == feedback.DetailId);
            if (orderDetail == null || orderDetail.IsReviewed)
            {
                TempData["Error"] = "Đơn hàng không hợp lệ hoặc đã được đánh giá.";
                return RedirectToAction("OrderDetails", "Profiles");
            }

            // 🔹 Lấy AccountId từ Session hoặc Identity
            var user = _context.Accounts.FirstOrDefault(a => a.Email == User.Identity.Name); // Nếu dùng Identity
            if (user == null)
            {
                TempData["Error"] = "Tài khoản không hợp lệ.";
                return RedirectToAction("OrderDetails", "Profiles");
            }

            feedback.AccountId = user.AccountId; // Gán đúng AccountId
            feedback.CreatedAt = DateTime.Now;

            _context.Feedbacks.Add(feedback);
            orderDetail.IsReviewed = true; // Cập nhật trạng thái đã đánh giá
            _context.SaveChanges();

            TempData["Success"] = "Cảm ơn bạn đã đánh giá sản phẩm!";
            return RedirectToAction("OrderDetails", "Profiles", new { id = orderDetail.OId });
        }

    }
}

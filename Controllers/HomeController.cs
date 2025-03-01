using System.Diagnostics;
using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClotherS.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;
        }

        public IActionResult Index()
        {        
            var products = _dataContext.Products.ToList();
            return View(products); 
        }
        public IActionResult Details(int id)
        {
            var product = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryName = product.Category?.CategoryName ?? "Unknown";
            ViewBag.BrandName = product.Brand?.BrandName ?? "Unknown";

            // Lấy danh sách feedback của sản phẩm
            var feedbacks = _dataContext.Feedbacks
                .Include(f => f.Account) // Nếu bạn muốn hiển thị tên người dùng
                .Where(f => f.ProductId == id && (f.Disable == false || f.Disable == null))
                .OrderByDescending(f => f.CreatedAt)
                .ToList();

            ViewBag.Feedbacks = feedbacks; 

            ViewBag.AverageRating = feedbacks.Any() ? feedbacks.Average(f => f.Rating) : 0;
            ViewBag.TotalReviews = feedbacks.Count;
            return View(product);
        }


        public IActionResult Privacy()
        {
            _logger.LogInformation("Privacy page được truy cập.");
            return View();
        }

        public IActionResult About()
        {
            // Thêm một trang About
            ViewData["Title"] = "Về chúng tôi";
            ViewData["Description"] = "ClotherS - Nơi cung cấp thời trang chất lượng.";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            _logger.LogError("Có lỗi xảy ra!");
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

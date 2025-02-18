using System.Diagnostics;
using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.AspNetCore.Mvc;

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

using System.Diagnostics;
using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

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

        public async Task<IActionResult> Index(string sortOrder, int page = 1)
        {
            int pageSize = 6; 
            ViewData["CurrentSort"] = sortOrder;

            var products = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Where(p => p.Category != null
                            && p.Brand != null
                            && !p.Category.Disable
                            && !p.Brand.Disable
                            && !p.Disable);

            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductId);
                    break;
            }

            var banners = await _dataContext.Banner.ToListAsync();
            ViewData["Banners"] = banners;

            var pagedProducts = await products.ToPagedListAsync(page, pageSize);

            return View(pagedProducts);
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

            var feedbacks = _dataContext.Feedbacks
                .Include(f => f.Account) 
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

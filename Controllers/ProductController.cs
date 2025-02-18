using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClotherS.Models;
using ClotherS.Repositories;

namespace ClotherS.Controllers
{
    public class ProductController : Controller
    {
        private readonly DataContext _context;

        public ProductController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Details(int id)
        {
            var product = _context.Products
                .Include(p => p.Category)  // Đảm bảo Category đã được load
                .Include(p => p.Brand)     // Đảm bảo Brand đã được load
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.CategoryName = product.Category?.CategoryName ?? "Unknown";
            ViewBag.BrandName = product.Brand?.BrandName ?? "Unknown";

            return View(product);
        }

    }
}

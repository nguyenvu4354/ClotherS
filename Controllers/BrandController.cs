using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClotherS.Repositories;

namespace ClotherS.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext context)
        {
            _dataContext = context;
        }

        public IActionResult Index(int id)
        {
            var brand = _dataContext.Brands
                .Include(b => b.Products)
                .FirstOrDefault(b => b.BrandId == id);

            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }
    }
}

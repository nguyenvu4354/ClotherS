
using Microsoft.AspNetCore.Mvc;
using ClotherS.Models;
using Newtonsoft.Json;
using ClotherS.Repositories;

namespace ClotherS.Controllers
{
    public class CartController : Controller
    {
        private readonly DataContext _context;

        public CartController(DataContext context)
        {
            _context = context;
        }

        private List<OrderDetail> GetCart()
        {
            var cart = HttpContext.Session.GetString("Cart");
            return cart == null ? new List<OrderDetail>() : JsonConvert.DeserializeObject<List<OrderDetail>>(cart);
        }

        // Lưu giỏ hàng vào Session
        private void SaveCart(List<OrderDetail> cart)
        {
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }

        // Thêm sản phẩm vào giỏ hàng
        public IActionResult AddToCart(int productId, int quantity)
        {
            var product = _context.Products.Find(productId);
            if (product == null) return NotFound();

            var cart = GetCart();

            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Add(new OrderDetail
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price,
                    Discount = product.Discount ?? 0
                });
            }

            SaveCart(cart);
            return RedirectToAction("Index");
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }
    }
}

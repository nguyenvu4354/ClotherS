using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace ClotherS.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly DataContext _context;

        public ShoppingCartController(DataContext context)
        {
            _context = context;
        }

        // Hiển thị giỏ hàng
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.AccountId == userId && o.IsCart);

            return View(cart ?? new Order { OrderDetails = new List<OrderDetail>() });
        }



        // Thêm sản phẩm vào giỏ hàng
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.AccountId == userId && o.IsCart);

            if (cart == null)
            {
                cart = new Order
                {
                    AccountId = userId.Value,
                    OrderDate = DateTime.Now,
                    IsCart = true,
                    Status = "Pending",
                    OrderDetails = new List<OrderDetail>()
                };
                _context.Orders.Add(cart);
            }

            var existingItem = cart.OrderDetails.FirstOrDefault(od => od.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var product = _context.Products.Find(productId);
                if (product != null)
                {
                    cart.OrderDetails.Add(new OrderDetail
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        Price = product.Price,
                        Discount = product.Discount ?? 0
                    });
                }
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // Xóa sản phẩm khỏi giỏ hàng
        public IActionResult RemoveFromCart(int productId)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.AccountId == userId && o.IsCart);

            if (cart != null)
            {
                var item = cart.OrderDetails.FirstOrDefault(od => od.ProductId == productId);
                if (item != null)
                {
                    cart.OrderDetails.Remove(item);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }

        // Xóa toàn bộ giỏ hàng
        public IActionResult ClearCart()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.AccountId == userId && o.IsCart);

            if (cart != null)
            {
                cart.OrderDetails.Clear();
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.AccountId == userId && o.IsCart);

            if (cart == null || !cart.OrderDetails.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            // Cập nhật trạng thái đơn hàng từ "giỏ hàng" thành "đã đặt hàng"
            cart.IsCart = false;
            cart.Status = "Processing";
            cart.OrderDate = DateTime.Now;

            _context.SaveChanges();

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("OrderConfirmation");
        }
        public IActionResult OrderConfirmation()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var latestOrder = _context.Orders
                .Where(o => o.AccountId == userId && !o.IsCart)
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault();

            if (latestOrder == null)
            {
                return RedirectToAction("Index");
            }

            return View(latestOrder);
        }


        // Lấy ID người dùng từ Claims (giả sử bạn đã có Authentication)
        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : (int?)null;
        }

    }
}

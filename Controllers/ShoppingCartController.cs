using ClotherS.Models;
using ClotherS.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                return RedirectToAction("Login", "Authentication");
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication");
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
                return RedirectToAction("Login", "Authentication");
            }

            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.AccountId == userId && o.IsCart);

            if (cart == null)
            {
                cart = new Order
                {
                    AccountId = userId.Value,
                    IsCart = true,
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
                        Discount = product.Discount ?? 0,
                        OrderDate = DateTime.Now,
                        Status = "Pending"
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
                return RedirectToAction("Login", "Authentication");
            }

            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.AccountId == userId && o.IsCart);

            if (cart != null)
            {
                var item = cart.OrderDetails.FirstOrDefault(od => od.ProductId == productId);
                if (item != null)
                {
                    _context.OrderDetails.Remove(item);
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
                return RedirectToAction("Login", "Authentication");
            }

            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.AccountId == userId && o.IsCart);

            if (cart != null)
            {
                _context.OrderDetails.RemoveRange(cart.OrderDetails);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Checkout(Order model)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.AccountId == userId && o.IsCart);

            if (cart == null || !cart.OrderDetails.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            // Cập nhật thông tin giao hàng từ form
            cart.ShippingAddress = model.ShippingAddress;
            cart.PhoneNumber = model.PhoneNumber;
            cart.CustomerNote = model.CustomerNote;

            foreach (var item in cart.OrderDetails)
            {
                item.Status = "Processing";
                item.OrderDate = DateTime.Now;
            }

            cart.IsCart = false;
            _context.SaveChanges();

            TempData["Success"] = "Order placed successfully!";
            return RedirectToAction("OrderConfirmation");
        }


        public IActionResult OrderConfirmation()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var latestOrder = _context.Orders
                .Where(o => o.AccountId == userId && !o.IsCart)
                .OrderByDescending(o => o.OrderDetails.Max(od => od.OrderDate))
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault();

            if (latestOrder == null)
            {
                return RedirectToAction("Index");
            }

            return View(latestOrder);
        }

        // Lấy ID người dùng từ Claims
        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : (int?)null;
        }

        public IActionResult ConfirmOrder()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            var user = _context.Users.FirstOrDefault(u => u.Id == int.Parse(userId));
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var cart = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.AccountId == user.Id && o.IsCart);

            if (cart == null)
            {
                return RedirectToAction("Index", "ShoppingCart");
            }

            // Gán dữ liệu từ User vào Order nếu chưa có
            cart.ShippingAddress ??= user.Address;
            cart.PhoneNumber ??= user.PhoneNumber;

            return View(cart);
        }


    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClotherS.Repositories;

namespace ClotherS.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DataContext _context;

        public OrdersController(DataContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Where(o => !o.IsCart) 
                .Include(o => o.Account)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Account)
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OId == id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int detailId, string status)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(detailId);
            if (orderDetail == null)
            {
                return NotFound();
            }

            // Nếu trạng thái đã là "Success" hoặc "Disable", không cho phép cập nhật nữa
            if (orderDetail.Status == "Success" || orderDetail.Status == "Disable")
            {
                return BadRequest("Không thể cập nhật trạng thái của sản phẩm đã hoàn thành hoặc bị vô hiệu hóa.");
            }

            // Danh sách trạng thái hợp lệ
            var validStatuses = new List<string> { "Processing", "Delivering", "Success", "Disable" };
            if (!validStatuses.Contains(status))
            {
                return BadRequest("Trạng thái không hợp lệ.");
            }

            orderDetail.Status = status;

            if (status == "Success")
            {
                orderDetail.ReceiveDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id = orderDetail.OId });
        }


    }
}

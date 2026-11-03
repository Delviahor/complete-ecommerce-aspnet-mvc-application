using eBooks.Data.Enums;
using eBooks.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly IOrdersService _ordersService;

        public AdminOrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        // Display all customer orders.
        public async Task<IActionResult> Index()
        {
            var orders = await _ordersService.GetAllOrdersAsync();

            return View(orders);
        }

        // Display details of a specific order.
        public async Task<IActionResult> Details(int id)
        {
            var order = await _ordersService
                .GetOrderDetailsForAdminAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // Update the status of an order.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int id,
            Order_Status newStatus)
        {
            var updated = await _ordersService
                .UpdateOrderStatusAsync(id, newStatus);

            if (!updated)
            {
                TempData["ErrorMessage"] =
                    "The order status could not be updated. " +
                    "The requested transition may not be allowed.";
            }
            else
            {
                TempData["SuccessMessage"] =
                    "Order status updated successfully.";
            }

            return RedirectToAction(
                nameof(Details),
                new { id });
        }
    }
}

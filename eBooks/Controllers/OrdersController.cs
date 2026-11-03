using eBooks.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eBooks.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrdersService _ordersService;

        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        // gets the current logged-in user's ID from the claims.
        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // Show the list of orders for the current logged-in user.
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Challenge();
            }

            var orders = await _ordersService.GetOrdersByUserIdAsync(userId);

            return View(orders);
        }

        // Show the details of a specific order for the current logged-in user.
        public async Task<IActionResult> Details(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Challenge();
            }

            var order = await _ordersService.GetOrderByIdAsync(id, userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // Cancel an order for the current logged-in user in pending status.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Challenge();
            }

            var cancelled = await _ordersService.CancelOrderAsync(id, userId);

            if (!cancelled)
            {
                TempData["ErrorMessage"] =
                    "The order could not be cancelled. It may no longer be pending.";
            }
            else
            {
                TempData["SuccessMessage"] =
                    "Your order has been cancelled successfully.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}


using eBooks.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eBooks.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // GET: /Cart
        // Displays the current user's shopping cart.
        public async Task<IActionResult> Index()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Challenge();
            }

            var cart = await _cartService.GetOrCreateCartAsync(userId);

            return View(cart);
        }

        // POST: /Cart/Add
        // Adds a book to the current user's cart.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int bookId, int quantity = 1)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Challenge();
            }

            try
            {
                await _cartService.AddItemAsync(userId, bookId, quantity);

                TempData["SuccessMessage"] =
                    "The book was added to your cart.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: /Cart/UpdateQuantity
        // Updates an item's quantity in the current user's cart.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(
            int cartItemId,
            int quantity)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Challenge();
            }

            try
            {
                await _cartService.UpdateQuantityAsync(
                    userId,
                    cartItemId,
                    quantity);

                TempData["SuccessMessage"] =
                    "The quantity was updated.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantityAjax(
            int cartItemId,
            int quantity)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Your session has expired."
                });
            }

            try
            {
                await _cartService.UpdateQuantityAsync(
                    userId,
                    cartItemId,
                    quantity);

                return Json(new { success = true });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // POST: /Cart/Remove
        // Removes an item from the current user's cart.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Challenge();
            }

            try
            {
                await _cartService.RemoveItemAsync(userId, cartItemId);

                TempData["SuccessMessage"] =
                    "The book was removed from your cart.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

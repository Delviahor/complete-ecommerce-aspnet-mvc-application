using eBooks.Data.Enums;
using eBooks.Data.Services;
using eBooks.ViewModels.Checkout;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eBooks.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IAddressesService _addressesService;
        private readonly IOrdersService _ordersService;

        public CheckoutController(
            ICartService cartService,
            IAddressesService addressesService,
            IOrdersService ordersService)
        {
            _cartService = cartService;
            _addressesService = addressesService;
            _ordersService = ordersService;
        }

        // Gets the ID of the authenticated user.
        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET: /Checkout
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Retrieves the user's cart.
            var cart = await _cartService.GetOrCreateCartAsync(userId);

            // You cannot start the checkout process with an empty cart.
            if (!cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // Retrieves only the user's addresses.
            var addresses = await _addressesService
                .GetAllByUserIdAsync(userId);

            // Build the view model.
            var model = new CheckoutViewModel
            {
                Items = cart.Items.Select(item =>
                    new CheckoutItemViewModel
                    {
                        BookId = item.BookId,
                        BookTitle = item.Book.Title,
                        CoverURLPicture = item.Book.CoverURLPicture,
                        UnitPrice = item.Book.Price,
                        Quantity = item.Quantity,

                        AvailableStock = item.Book.InStock,

                        IsAvailable =
                            item.Book.Status == Book_Status.Published &&
                            item.Book.InStock >= item.Quantity
                    }).ToList(),

                Addresses = addresses.Select(address =>
                    new CheckoutAddressViewModel
                    {
                        AddressId = address.AddressId,
                        RecipientName = address.RecipientName,
                        Phone = address.Phone,
                        Street = address.Street,
                        Number = address.Number,
                        Neighborhood = address.Neighborhood,
                        City = address.City,
                        State = address.State,
                        PostalCode = address.PostalCode,
                        References = address.References
                    }).ToList()
            };

            return View(model);
        }

        // GET: /Checkout/Confirmation/5
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var order = await _ordersService.GetOrderByIdAsync(id, userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: /Checkout/Confirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int? selectedAddressId)
        {
            var userId = GetUserId();

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Validate that an address has been selected.
            if (selectedAddressId == null || selectedAddressId <= 0)
            {
                TempData["Error"] = "You must select a shipping address.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Create the order and obtain its identifier.
                var orderId = await _ordersService.CreateOrderAsync(
                    userId,
                    selectedAddressId.Value);

                return RedirectToAction(
                    nameof(Confirmation),
                    new { id = orderId });
            }
            catch (InvalidOperationException ex)
            {
                // Display validation errors for the cart or the address.
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}


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

        // Obtiene el ID del usuario autenticado.
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

            // Obtiene el carrito del usuario.
            var cart = await _cartService.GetOrCreateCartAsync(userId);

            // No se puede iniciar el checkout con un carrito vacío.
            if (!cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            // Obtiene únicamente las direcciones del usuario.
            var addresses = await _addressesService
                .GetAllByUserIdAsync(userId);

            // Construye el modelo de vista.
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

            // Validar que se haya seleccionado una dirección.
            if (selectedAddressId == null || selectedAddressId <= 0)
            {
                TempData["Error"] = "You must select a shipping address.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Crear la orden y obtener su identificador.
                var orderId = await _ordersService.CreateOrderAsync(
                    userId,
                    selectedAddressId.Value);

                return RedirectToAction(
                    nameof(Confirmation),
                    new { id = orderId });
            }
            catch (InvalidOperationException ex)
            {
                // Mostrar errores de validación del carrito o la dirección.
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

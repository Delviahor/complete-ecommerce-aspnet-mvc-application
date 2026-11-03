
using eBooks.Data.Services;
using eBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using eBooks.ViewModels;


namespace eBooks.Controllers
{
    [Authorize]
    public class AddressesController : Controller
    {
        private readonly IAddressesService _addressesService;

        public AddressesController(IAddressesService addressesService)
        {
            _addressesService = addressesService;
        }

        // Obtiene el ID del usuario autenticado.
        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // GET: /Addresses
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Challenge();
            }

            var addresses = await _addressesService
                .GetAllByUserIdAsync(userId);

            return View(addresses);
        }


        // GET: /Addresses/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateAddressViewModel
            {
                RecipientName = string.Empty,
                Phone = string.Empty,
                Street = string.Empty,
                Number = string.Empty,
                Neighborhood = string.Empty,
                City = string.Empty,
                State = string.Empty,
                PostalCode = string.Empty
            });
        }

        // POST: /Addresses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAddressViewModel model)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Challenge();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var address = new Address
            {
                UserId = userId,
                RecipientName = model.RecipientName,
                Phone = model.Phone,
                Street = model.Street,
                Number = model.Number,
                Neighborhood = model.Neighborhood,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode,
                References = model.References
            };

            await _addressesService.AddAsync(address);

            TempData["SuccessMessage"] = "Address added successfully.";

            return RedirectToAction(nameof(Index));
        }




        // GET: /Addresses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Challenge();
            }

            var address = await _addressesService.GetByIdAsync(id, userId);

            if (address == null)
            {
                return NotFound();
            }

            // Convert address entity to EditAddressViewModel
            var model = new EditAddressViewModel
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
            };

            return View(model);
        }

        // POST: /Addresses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditAddressViewModel model)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Challenge();
            }

            if (id != model.AddressId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // converting the EditAddressViewModel back to Address entity
            var updatedAddress = new Address
            {
                UserId = userId,
                RecipientName = model.RecipientName,
                Phone = model.Phone,
                Street = model.Street,
                Number = model.Number,
                Neighborhood = model.Neighborhood,
                City = model.City,
                State = model.State,
                PostalCode = model.PostalCode,
                References = model.References
            };

            var updated = await _addressesService.UpdateAsync(
                id, userId, updatedAddress);

            if (!updated)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Address updated successfully.";

            return RedirectToAction(nameof(Index));
        }


        // POST: /Addresses/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return Challenge();
            }

            var deleted = await _addressesService.DeleteAsync(id, userId);

            if (!deleted)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Address deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}

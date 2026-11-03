using eBooks.Data.Services;
using eBooks.ViewModels.BookReviews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eBooks.Controllers
{
    public class BookReviewsController : Controller
    {
        private readonly IBookReviewsService _bookReviewsService;

        public BookReviewsController(IBookReviewsService bookReviewsService)
        {
            _bookReviewsService = bookReviewsService;
        }

        // Muestra las reseñas publicadas de un libro.
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ByBook(int bookId)
        {
            var reviews = await _bookReviewsService
                .GetPublishedByBookIdAsync(bookId);

            return View(reviews);
        }

        // Crea una nueva reseña.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateBookReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(
                    "Details",
                    "Catalog",
                    new { id = model.BookId });
            }

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Challenge();

            try
            {
                await _bookReviewsService.CreateAsync(
                    userId,
                    model.BookId,
                    model.Rating,
                    model.Content);

                TempData["Success"] =
                    "Your review has been submitted for moderation.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(
                "Details",
                "Catalog",
                new { id = model.BookId });
        }

        // Muestra el formulario para editar una reseña propia.
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int bookId)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var review = await _bookReviewsService
                .GetByUserAndBookAsync(userId, bookId);

            if (review == null)
                return NotFound();

            var model = new EditBookReviewViewModel
            {
                BookReviewId = review.BookReviewId,
                BookId = review.BookId,
                Rating = review.Rating,
                Content = review.Content
            };

            return View(model);
        }

        // Actualiza una reseña propia.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            EditBookReviewViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Challenge();

            // Recuperamos la reseña asociada al usuario autenticado.
            var review = await _bookReviewsService
                .GetByUserAndBookAsync(userId, model.BookId);

            // Comprobamos que el ID enviado corresponda
            // realmente a esa reseña.
            if (review == null ||
                review.BookReviewId != model.BookReviewId)
            {
                return NotFound();
            }

            try
            {
                await _bookReviewsService.UpdateAsync(
                    review.BookReviewId,
                    userId,
                    model.Rating,
                    model.Content);

                TempData["Success"] =
                    "Your review has been updated and submitted for moderation.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }

            return RedirectToAction(
                "Details",
                "Catalog",
                new { id = model.BookId });
        }
    }
}

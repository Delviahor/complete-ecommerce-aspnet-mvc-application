using eBooks.Data.Services;
using eBooks.ViewModels.Catalog;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eBooks.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IBooksService _booksService;
        private readonly IBookReviewsService _bookReviewsService;

        public CatalogController(IBooksService booksService, IBookReviewsService bookReviewsService)
        {
            _booksService = booksService;
            _bookReviewsService = bookReviewsService;
        }

        // GET: /Catalog
        public async Task<IActionResult> Index()
        {
            var books = await _booksService.GetCatalogAsync();

            return View(books);
        }

        // GET: /Catalog/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var book = await _booksService.GetPublishedByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }

            // Retrieves only the approved reviews for the book.
            var reviews = await _bookReviewsService
                .GetPublishedByBookIdAsync(id);

            // Gets the ID of the authenticated user, if it exists.
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Checks whether the user has already posted a review or has one pending.
            var currentUserReview = string.IsNullOrEmpty(userId)
                ? null
                : await _bookReviewsService.GetByUserAndBookAsync(userId, id);

            var model = new CatalogBookDetailsViewModel
            {
                Book = book,
                Reviews = reviews,
                CurrentUserReview = currentUserReview
            };

            return View(model);
        }
    }
}

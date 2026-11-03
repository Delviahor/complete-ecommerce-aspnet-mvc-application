using eBooks.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IBooksService _booksService;

        public CatalogController(IBooksService booksService)
        {
            _booksService = booksService;
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

            return View(book);
        }
    }
}

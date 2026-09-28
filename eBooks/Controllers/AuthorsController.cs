using eBooks.Models;
using eBooks.Data.Services;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IAuthorsService _authorsService;

        public AuthorsController(IAuthorsService authorsService)
        {
            _authorsService = authorsService;
        }

        public async Task<IActionResult> Index()
        {
            var authors = await _authorsService.GetAllAuthorsAsync();

            return View(authors);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}
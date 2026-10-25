using eBooks.Data.Services;
using eBooks.Models;
using eBooks.ViewModels.Authors;
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

            return View("Index", authors);
        }

        //HTTP GET: Authors/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        //HTTP POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAuthorViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", viewModel);
            }

            var author = new Author
            {
                FullName = viewModel.FullName,
                Biography = viewModel.Biography,
                ProfileURLPicture = viewModel.ProfileURLPicture
            };

            await _authorsService.AddAsync(author);

            return RedirectToAction(nameof(Index));
        }

        //HTTP GET: Authors/Edit/n
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var author = await _authorsService.GetByIdAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            var viewModel = new EditAuthorViewModel
            {
                Id = author.AuthorId,
                FullName = author.FullName,
                Biography = author.Biography,
                ProfileURLPicture = author.ProfileURLPicture
            };
            return View(viewModel);
        }

        //HTTP POST: Authors/Edit/n
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditAuthorViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var author = new Author
            {
                AuthorId = model.Id,
                FullName = model.FullName,
                Biography = model.Biography,
                ProfileURLPicture = model.ProfileURLPicture
            };

            try
            {
                await _authorsService.UpdateAsync(id, author);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
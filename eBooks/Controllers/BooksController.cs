using eBooks.Data.Enums;
using eBooks.Data.Services;
using eBooks.Models;
using eBooks.ViewModels.Books;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eBooks.Controllers
{
    public class BooksController : Controller
    {
        private readonly IBooksService _booksService;
        private readonly ICategoriesService _categoriesService;
        private readonly IPublishersService _publishersService;
        private readonly IAuthorsService _authorsService;


        public BooksController(

            IBooksService booksService,
            IAuthorsService authorsService,
            ICategoriesService categoriesService,
            IPublishersService publishersService)
        {
            _booksService = booksService;
            _authorsService = authorsService;
            _categoriesService = categoriesService;
            _publishersService = publishersService;
        }
        //GET: create Books
        public async Task<IActionResult> Create()
        {
            var model = new CreateBookViewModel();

            model.Categories = (await _categoriesService.GetAllCategoriesAsync())
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                });

            model.Publishers = (await _publishersService.GetAllPublishersAsync())
                .Select(p => new SelectListItem
                {
                    Value = p.PublisherId.ToString(),
                    Text = p.Name
                });

            model.Authors = (await _authorsService.GetAllAuthorsAsync())
                .Select(a => new SelectListItem
                {
                    Value = a.AuthorId.ToString(),
                    Text = a.FullName
                });

            return View(model);
        }

        // POST: create book
        [HttpPost]
        public async Task<IActionResult> Create(CreateBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = (await _categoriesService.GetAllCategoriesAsync())
                    .Select(c => new SelectListItem
                    {
                        Value = c.CategoryId.ToString(),
                        Text = c.Name
                    });

                model.Publishers = (await _publishersService.GetAllPublishersAsync())
                    .Select(p => new SelectListItem
                    {
                        Value = p.PublisherId.ToString(),
                        Text = p.Name
                    });

                model.Authors = (await _authorsService.GetAllAuthorsAsync())
                    .Select(a => new SelectListItem
                    {
                        Value = a.AuthorId.ToString(),
                        Text = a.FullName
                    });

                return View(model);
            }

            var book = new Book
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                Pages = model.Pages,
                PublicationDate = model.PublicationDate,
                CoverURLPicture = model.CoverURLPicture,
                InStock = model.InStock,
                Language = model.Language,
                Format = model.Format,
                CategoryId = model.CategoryId,
                PublisherId = model.PublisherId
            };

            foreach (var authorId in model.AuthorIds)
            {
                book.AuthorBooks.Add(new Author_Book
                {
                    AuthorId = authorId
                });
            }
            Console.WriteLine($"PublisherId: {model.PublisherId}");
            Console.WriteLine($"CategoryId: {model.CategoryId}");
            Console.WriteLine($"Autores: {string.Join(", ", model.AuthorIds)}");

            await _booksService.AddAsync(book);

            return RedirectToAction(nameof(Index));
        }

        

        public async Task<IActionResult> Index()
        {
            var books = await _booksService.GetAllAsync();

            return View(books);
        }

        //GET: edit book
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _booksService.GetByIdAsync(id);

            if (book == null)
            {
                return NotFound();
            }


            var model = new EditBookViewModel
            {
                BookId = book.BookId,
                Title = book.Title,
                Description = book.Description,
                Price = book.Price,
                CategoryId = book.CategoryId,
                CoverURLPicture = book.CoverURLPicture,


                // new properties for editing

                Language = book.Language,
                Format = book.Format,
                Status = book.Status,
                InStock = book.InStock,
                Pages = book.Pages,
                PublisherId = book.PublisherId,

                AuthorIds = book.AuthorBooks
                .Select(ab => ab.AuthorId)
                .ToList(),

                Categories = (await _categoriesService.GetAllCategoriesAsync())
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.Name
                }),

                Publishers = (await _publishersService.GetAllPublishersAsync())
                .Select(p => new SelectListItem
                {
                    Value = p.PublisherId.ToString(),
                    Text = p.Name
                }),

                // ...
                Authors = (await _authorsService.GetAllAuthorsAsync())
                .Select(a => new SelectListItem
                {
                    Value = a.AuthorId.ToString(),
                    Text = a.FullName,
                    Selected = book.AuthorBooks.Any(ab => ab.AuthorId == a.AuthorId)
                })

            };


            return View(model);
        }

        //POST: edit book
        [HttpPost]
        public async Task<IActionResult> Edit(EditBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var book = new Book
            {
                Title = model.Title,
                Description = model.Description,
                Price = model.Price,
                CategoryId = model.CategoryId,
                CoverURLPicture = model.CoverURLPicture,

                // new properties for editing

                PublisherId = model.PublisherId,
                Language = model.Language,
                Format = model.Format,
                Status = model.Status,
                InStock = model.InStock,
                Pages = model.Pages
            };

            Console.WriteLine("Autores seleccionados:");
            Console.WriteLine(string.Join(",", model.AuthorIds));

            //Console.WriteLine(model.CategoryId);
            await _booksService.UpdateAsync(model.BookId, book, model.AuthorIds ?? new List<int>());


            return RedirectToAction(nameof(Index));
        }

    }
}

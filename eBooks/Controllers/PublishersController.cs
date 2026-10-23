using eBooks.Data.Services;
using eBooks.Models;
using eBooks.ViewModels.Authors;
using eBooks.ViewModels.Publishers;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.Controllers
{
    public class PublishersController: Controller
    {
        private readonly IPublishersService _publishersService;

        public PublishersController(IPublishersService publishersService)
        {
            _publishersService = publishersService;
        }
        public async Task<IActionResult> Index()
        {
            var publishers = await _publishersService.GetAllPublishersAsync();

            return View("Index", publishers);
        }

        //HTTP GET: Publishers/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        //HTTP POST: publishers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Publisher publisher)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", publisher);
            }
            await _publishersService.AddAsync(publisher);
            return RedirectToAction(nameof(Index));
        }

        //HTTP GET: Publishers/Edit/n
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var publisher = await _publishersService.GetByIdAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }

            var viewModel = new EditPublisherViewModel
            {
                Id = publisher.PublisherId,
                Name = publisher.Name,
                ProfileURLPicture = publisher.ProfileURLPicture
            };
            return View(viewModel);
        }

        //HTTP POST: Publishers/Edit/n
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditPublisherViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var publisher = new Publisher
            {
                PublisherId = model.Id,
                Name = model.Name,
                ProfileURLPicture = model.ProfileURLPicture
            };

            try
            {
                await _publishersService.UpdateAsync(id, publisher);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

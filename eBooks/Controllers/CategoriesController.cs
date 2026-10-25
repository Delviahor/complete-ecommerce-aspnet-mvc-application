using eBooks.Data.Services;
using eBooks.Models;
using eBooks.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoriesService _categoriesService;

        public CategoriesController(ICategoriesService categoriesService)
        {
            _categoriesService = categoriesService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoriesService.GetAllCategoriesAsync();

            return View("Index", categories);
        }

        // HTTP GET: Categories/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View("Create");
        }

        // HTTP POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }

            var category = new Category
            {
                Name = model.Name.Trim()
            };

            await _categoriesService.AddAsync(category);

            return RedirectToAction(nameof(Index));
        }
        //HTTP GET: Categories/Edit/n
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoriesService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            var viewModel = new EditCategoryViewModel
            {
                Id = category.CategoryId,
                Name = category.Name,
                
            };
            return View("Edit", viewModel);
        }

        //HTTP POST: Categories/Edit/n
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCategoryViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }
            var category = new Category
            {
                CategoryId = model.Id,
                Name = model.Name.Trim()
            };
            try
            {
                await _categoriesService.UpdateAsync(id, category);
            }
            catch (Exception ex)
            {
                // Log the exception (ex) here if needed
                ModelState.AddModelError(string.Empty, "An error occurred while updating the category.");
                return View("Edit", model);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

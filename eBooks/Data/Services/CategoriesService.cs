using eBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Data.Services
{
    public class CategoriesService : ICategoriesService
    {
        private readonly AppDbContext _context;
        public CategoriesService(AppDbContext context)
        {
            _context = context;
        }

        // Adds a new category to the database and persists the changes.
        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);

            await _context.SaveChangesAsync();
        }

        // Retrieves all categories from the database.
        // Used to populate category lists in book management forms.
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // Retrieves a category by its ID.
        // Returns null if no matching category is found.
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        // Retrieves the existing category that will be updated.
        public async Task UpdateAsync(int id, Category category)
        {
            var existingCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            // Throws an exception if the specified category does not exist.
            if (existingCategory == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            // Updates the category name with the provided value.
            existingCategory.Name = category.Name;

            await _context.SaveChangesAsync();
        }
    }
}

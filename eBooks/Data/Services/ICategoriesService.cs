using eBooks.Models;

namespace eBooks.Data.Services
{
    /// <summary>
    /// Defines the operations available for managing book categories.
    /// </summary>
    public interface ICategoriesService
    {
        /// <summary>
        /// Retrieves all book categories.
        /// </summary>
        /// <returns>A collection of all categories.</returns>
        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        /// <summary>
        /// Retrieves a category by its ID.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <returns>The category if found; otherwise, null.</returns>
        Task<Category?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new book category.
        /// </summary>
        /// <param name="category">The category to add.</param>
        Task AddAsync(Category category);

        /// <summary>
        /// Updates an existing book category.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="category">The updated category data.</param>
        Task UpdateAsync(int id, Category category);
    }
}

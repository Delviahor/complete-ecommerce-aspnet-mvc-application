using eBooks.Models;

namespace eBooks.Data.Services
{
    /// <summary>
    /// Defines the operations available for managing authors.
    /// </summary>
    public interface IAuthorsService
    {
        /// <summary>
        /// Retrieves all authors from the database.
        /// </summary>
        /// <returns>A collection of authors.</returns>
        Task<IEnumerable<Author>> GetAllAuthorsAsync();

        /// <summary>
        /// Adds a new author to the database.
        /// </summary>
        /// <param name="author">The author information to be saved.</param>
        Task AddAsync(Author author);

        /// <summary>
        /// Retrieves an author by its ID.
        /// </summary>
        /// <param name="id">The ID of the author.</param>
        /// <returns>The author if found; otherwise, null.</returns>
        Task<Author?> GetByIdAsync(int id);

        /// <summary>
        /// Updates an existing author's information.
        /// </summary>
        /// <param name="id">The ID of the author to update.</param>
        /// <param name="author">The updated author information.</param>
        Task UpdateAsync(int id, Author author);
    }
}

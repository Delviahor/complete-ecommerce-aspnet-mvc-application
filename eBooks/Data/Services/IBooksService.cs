using eBooks.Data.Enums;
using eBooks.Models;

namespace eBooks.Data.Services
{
    /// <summary>
    /// Defines the operations available for managing books,
    /// including catalog retrieval, creation, updates, and status management.
    /// </summary>
    public interface IBooksService
    {
        /// <summary>
        /// Retrieves books available for public catalog display.
        /// Only Published and OutOfStock books are included.
        /// </summary>
        /// <returns>A collection of publicly visible books.</returns>
        Task<IEnumerable<Book>> GetCatalogAsync();

        /// <summary>
        /// Retrieves all books regardless of their publication status.
        /// Intended for administrative book management.
        /// </summary>
        /// <returns>A collection containing all books.</returns>
        Task<IEnumerable<Book>> GetAllAsync();

        /// <summary>
        /// Retrieves a book by its ID, regardless of its publication status.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <returns>The book if found; otherwise, null.</returns>
        Task<Book?> GetByIdAsync(int id);

        /// <summary>
        /// Adds a new book to the database.
        /// New books are assigned Draft status by default.
        /// </summary>
        /// <param name="book">The book information to be saved.</param>
        Task AddAsync(Book book);

        /// <summary>
        /// Updates an existing book and its associated authors.
        /// </summary>
        /// <param name="id">The ID of the book to update.</param>
        /// <param name="book">The updated book information.</param>
        /// <param name="authorIds">The IDs of the authors associated with the book.</param>
        Task UpdateAsync(int id, Book book, List<int> authorIds);

        /// <summary>
        /// Changes the publication or availability status of a book.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <param name="status">The new book status.</param>
        Task ChangeStatusAsync(int id, Book_Status status);

        /// <summary>
        /// Retrieves a specific book only if it is publicly visible.
        /// Published and OutOfStock books are included.
        /// </summary>
        /// <param name="id">The ID of the book.</param>
        /// <returns>The book if publicly visible; otherwise, null.</returns>
        Task<Book?> GetPublishedByIdAsync(int id);
    }
}

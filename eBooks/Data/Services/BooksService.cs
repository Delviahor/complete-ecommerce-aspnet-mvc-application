using eBooks.Data.Enums;
using eBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Data.Services
{
    public class BooksService : IBooksService
    {
        private readonly AppDbContext _context;

        public BooksService(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Book book)
        {
            // New books are created as Draft by default.
            // They must be explicitly published before appearing in the public catalog.
            book.Status = Book_Status.Draft;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusAsync(int id, Book_Status status)
        {
            // Retrieves the book whose publication status will be changed.
            var book = await _context.Books.FindAsync(id);
            // Stops the operation if the specified book does not exist.
            if (book == null)
            {
                return;
            }
            // Applies the new status and persists the changes.
            book.Status = status;
            await _context.SaveChangesAsync();
        }

        // Retrieves all books, including their category, publisher,
        // and associated authors.
        // Includes related entities to provide complete information
        // for administrative views and book management.
        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .Include(b => b.AuthorBooks)
            .ThenInclude(ab => ab.Author)
            .ToListAsync();
        
        }
        // Retrieves a book by its ID, including its associated authors.
        // Returns null if no matching book is found.
        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
            .Include(b => b.AuthorBooks)
            .ThenInclude(ab => ab.Author)
            .FirstOrDefaultAsync(b => b.BookId == id);
        }

        // Retrieves books available for public catalog display.
        // Only Published and OutOfStock books are included.
        // Draft and Discontinued books remain hidden from regular users.
        // Related category, publisher, and author information is loaded
        // to provide complete catalog entries.
        // Results are ordered alphabetically by title.
        public async Task<IEnumerable<Book>> GetCatalogAsync()
        {
            return await _context.Books
            .Where(b =>
                b.Status == Book_Status.Published ||
                b.Status == Book_Status.OutOfStock)
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .Include(b => b.AuthorBooks)
                .ThenInclude(ab => ab.Author)
            .OrderBy(b => b.Title)
            .ToListAsync();
        }
        // Retrieves a specific book only if it is publicly visible.
        // Published and OutOfStock books can be accessed by regular users.
        // Draft and Discontinued books are excluded.
        // Includes category, publisher, and author information
        // to provide complete details for the book's public page.
        public async Task<Book?> GetPublishedByIdAsync(int id)
        {
            return await _context.Books
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .Include(b => b.AuthorBooks)
            .ThenInclude(ab => ab.Author)
            .FirstOrDefaultAsync(b =>
                b.BookId == id &&
                (b.Status == Book_Status.Published ||
                 b.Status == Book_Status.OutOfStock));
        }
        // Retrieves the existing book along with its author relationships.
        // The existing entity is tracked by EF Core so its properties
        // and relationships can be updated.
        public async Task UpdateAsync(int id, Book book, List<int> authorIds)
        {
            var existingBook = await _context.Books
            .Include(b => b.AuthorBooks)
            .FirstOrDefaultAsync(b => b.BookId == id);
            // Throws an exception if the specified book does not exist.
            if (existingBook == null)
            {
                throw new Exception("Book not found");
            }

            // Updates the book's basic information and publishing details.
            existingBook.Title = book.Title;
            existingBook.Description = book.Description;
            existingBook.Price = book.Price;
            existingBook.CategoryId = book.CategoryId;
            existingBook.CoverURLPicture = book.CoverURLPicture;

           
            existingBook.Language = book.Language;
            existingBook.Format = book.Format;
            existingBook.Status = book.Status;
            existingBook.InStock = book.InStock;
            existingBook.Pages= book.Pages;
            existingBook.PublisherId = book.PublisherId;


            //update the author relationships for the book:
            // Replaces the existing author relationships with the new selection.
            // Existing associations are removed before adding the updated list.
            // This ensures that the book reflects exactly the authors
            // selected in the editing form.
            _context.Author_Books.RemoveRange(existingBook.AuthorBooks);

            foreach (var authorId in authorIds)
            {
                existingBook.AuthorBooks.Add(new Author_Book
                {
                    BookId = existingBook.BookId,
                    AuthorId = authorId
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}

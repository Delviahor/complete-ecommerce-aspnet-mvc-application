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
            book.Status = Book_Status.Draft;
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusAsync(int id, Book_Status status)
        {
            var book = await _context.Books.FindAsync(id);

            if (book == null)
            {
                return;
            }
            book.Status = status;
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
            .Include(b => b.Category)
            .Include(b => b.Publisher)
            .Include(b => b.AuthorBooks)
            .ThenInclude(ab => ab.Author)
            .ToListAsync();
        
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
            .Include(b => b.AuthorBooks)
            .ThenInclude(ab => ab.Author)
            .FirstOrDefaultAsync(b => b.BookId == id);
        }

        public async Task<IEnumerable<Book>> GetCatalogAsync()
        {
            return await _context.Books
            .Where(b =>
                b.Status == Book_Status.Published ||
                b.Status == Book_Status.OutOfStock)
            .ToListAsync();
        }

        public async Task UpdateAsync(int id, Book book, List<int> authorIds)
        {
            var existingBook = await _context.Books
            .Include(b => b.AuthorBooks)
            .FirstOrDefaultAsync(b => b.BookId == id);

            if (existingBook == null)
            {
                throw new Exception("Book not found");
            }


            existingBook.Title = book.Title;
            existingBook.Description = book.Description;
            existingBook.Price = book.Price;
            existingBook.CategoryId = book.CategoryId;
            existingBook.CoverURLPicture = book.CoverURLPicture;

            // Update other properties as needed
            existingBook.Language = book.Language;
            existingBook.Format = book.Format;
            existingBook.Status = book.Status;
            existingBook.InStock = book.InStock;
            existingBook.Pages= book.Pages;
            existingBook.PublisherId = book.PublisherId;

            

            // Actualizar autores
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

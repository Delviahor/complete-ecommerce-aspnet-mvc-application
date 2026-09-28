using eBooks.Data.Enums;
using eBooks.Models;

namespace eBooks.Data.Services
{
    public interface IBooksService
    {
        Task<IEnumerable<Book>> GetCatalogAsync();
        Task<IEnumerable<Book>> GetAllAsync();

        Task<Book?> GetByIdAsync(int id);

        Task AddAsync(Book book);

        Task UpdateAsync(int id, Book book, List<int> authorIds);

        Task ChangeStatusAsync(int id, Book_Status status);
    }
}

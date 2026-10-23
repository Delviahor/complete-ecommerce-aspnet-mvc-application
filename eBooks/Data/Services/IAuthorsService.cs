using eBooks.Models;

namespace eBooks.Data.Services
{
    public interface IAuthorsService
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();
        Task AddAsync(Author author);
        Task<Author?> GetByIdAsync(int id);
        Task UpdateAsync(int id, Author author);
    }
}

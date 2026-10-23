using eBooks.Models;

namespace eBooks.Data.Services
{
    public interface IPublishersService
    {
        Task<IEnumerable<Publisher>> GetAllPublishersAsync();
        Task<Publisher?> GetByIdAsync(int id);
        Task AddAsync(Publisher publisher);
        Task UpdateAsync(int id, Publisher publisher);
    }
}

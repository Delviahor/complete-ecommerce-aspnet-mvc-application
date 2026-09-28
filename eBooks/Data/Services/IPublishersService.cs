using eBooks.Models;

namespace eBooks.Data.Services
{
    public interface IPublishersService
    {
        Task<IEnumerable<Publisher>> GetAllPublishersAsync();
    }
}

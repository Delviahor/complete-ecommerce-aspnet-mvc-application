using eBooks.Models;

namespace eBooks.Data.Services
{
    public interface IAuthorsService
    {
        Task<IEnumerable<Author>> GetAllAuthorsAsync();

    }
}

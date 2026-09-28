using eBooks.Models;

namespace eBooks.Data.Services
{
    public interface ICategoriesService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();

    }
}

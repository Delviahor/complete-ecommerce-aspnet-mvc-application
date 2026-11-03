using eBooks.Models;

namespace eBooks.Data.Services
{
    public interface ICartService
    {
        Task<Cart> GetOrCreateCartAsync(string userId);

        Task AddItemAsync(string userId, int bookId, int quantity);

        Task UpdateQuantityAsync(
            string userId,
            int cartItemId,
            int quantity);

        Task RemoveItemAsync(string userId, int cartItemId);
    }
}
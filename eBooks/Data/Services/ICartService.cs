using eBooks.Models;

namespace eBooks.Data.Services
{
    /// <summary>
    /// Defines the operations available for managing user shopping carts.
    /// </summary>
    public interface ICartService
    {
        /// <summary>
        /// Retrieves the user's shopping cart or creates one if it does not exist.
        /// </summary>
        /// <param name="userId">The ID of the cart owner.</param>
        /// <returns>The user's shopping cart with its associated items.</returns>
        Task<Cart> GetOrCreateCartAsync(string userId);

        /// <summary>
        /// Adds a book to the user's cart or increases its existing quantity.
        /// Only published books with available stock can be added.
        /// The quantity cannot exceed 10 units per book or the available stock.
        /// </summary>
        /// <param name="userId">The ID of the cart owner.</param>
        /// <param name="bookId">The ID of the book to add.</param>
        /// <param name="quantity">The quantity of books to add.</param>
        Task AddItemAsync(string userId, int bookId, int quantity);

        /// <summary>
        /// Updates the quantity of an existing cart item.
        /// The item must belong to the specified user and have sufficient stock.
        /// </summary>
        /// <param name="userId">The ID of the cart owner.</param>
        /// <param name="cartItemId">The ID of the cart item to update.</param>
        /// <param name="quantity">The new quantity.</param>
        Task UpdateQuantityAsync(
            string userId,
            int cartItemId,
            int quantity);

        /// <summary>
        /// Removes an item from the user's shopping cart.
        /// </summary>
        /// <param name="userId">The ID of the cart owner.</param>
        /// <param name="cartItemId">The ID of the cart item to remove.</param>
        Task RemoveItemAsync(string userId, int cartItemId);
    }
}
using eBooks.Data.Enums;
using eBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Data.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            // Retrieves the user's cart along with its items and related book information.
            // Related entities are included to provide all the information needed
            // to display the cart without requiring additional queries.
            var cart = await _context.Carts
                .Include(c => c.Items)
                    .ThenInclude(i => i.Book)
                        .ThenInclude(b => b.Category)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Book)
                        .ThenInclude(b => b.Publisher)
                .Include(c => c.Items)
                    .ThenInclude(i => i.Book)
                        .ThenInclude(b => b.AuthorBooks)
                            .ThenInclude(ab => ab.Author)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // Creates a new cart if the user does not already have one.
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };

                // Persists the newly created cart to the database.
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                // Reloads the cart with its items and associated books
                // after the new cart has been saved.
                cart = await _context.Carts
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Book)
                    .FirstAsync(c => c.UserId == userId);
            }

            return cart;
        }

        public async Task AddItemAsync(
            string userId,
            int bookId,
            int quantity)
        {
            // Validates the requested quantity.
            // Each cart item must contain between 1 and 10 units.
            if (quantity < 1 || quantity > 10)
            {
                throw new InvalidOperationException(
                    "The items must be between 1 and 10 units");
            }
            // Retrieves the book that the user wants to add to the cart.
            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.BookId == bookId);

            // Prevents adding a book that does not exist in the database.
            if (book == null)
            {
                throw new InvalidOperationException(
                    "Book not found.");
            }
            // Only published books with available stock can be added to the cart.
            // OutOfStock, Draft, and Discontinued books are not eligible.
            if (book.Status != Book_Status.Published
                || book.InStock <= 0)
            {
                throw new InvalidOperationException(
                    "This book is not availabe.");
            }
            // Retrieves the user's existing cart or creates one if necessary.
            var cart = await GetOrCreateCartAsync(userId);

            // Checks whether the selected book is already present in the cart.
            var item = await _context.CartItems
                .FirstOrDefaultAsync(i =>
                    i.CartId == cart.CartId &&
                    i.BookId == bookId);
            // Calculates the resulting quantity, taking existing cart items into account.
            int newQuantity = quantity;

            if (item != null)
            {
                newQuantity += item.Quantity;
            }

            // Ensures that the combined quantity does not exceed the maximum
            // allowed number of units per book.
            if (newQuantity > 10)
            {
                throw new InvalidOperationException(
                    "You cannot add more than 10 copies of the same book.");
            }

            // Prevents users from requesting more units than the available stock.
            if (newQuantity > book.InStock)
            {
                throw new InvalidOperationException(
                    $"Only {book.InStock} are available");
            }

            // Creates a new cart item if the book is not already in the cart.
            // Otherwise, updates the quantity of the existing item.
            if (item == null)
            {
                _context.CartItems.Add(new CartItem
                {
                    CartId = cart.CartId,
                    BookId = bookId,
                    Quantity = quantity
                });
            }
            else
            {
                item.Quantity = newQuantity;
            }

            await _context.SaveChangesAsync();
        }

        // Validates the requested quantity to enforce the cart's quantity limits.
        public async Task UpdateQuantityAsync(
            string userId,
            int cartItemId,
            int quantity)
        {
            if (quantity < 1 || quantity > 10)
            {
                throw new InvalidOperationException(
                    "The quantity must be between 1 and 10.");
            }

            // Retrieves the cart item only if it belongs to the specified user.
            // Includes the cart and book to verify ownership and check availability.
            var item = await _context.CartItems
                .Include(i => i.Cart)
                .Include(i => i.Book)
                .FirstOrDefaultAsync(i =>
                    i.CartItemId == cartItemId &&
                    i.Cart.UserId == userId);

            // Prevents users from modifying items that do not exist
            // or belong to another user's cart.
            if (item == null)
            {
                throw new InvalidOperationException(
                    "The item does not exist in your cart.");
            }

            // Revalidates book availability because its status or stock
            // may have changed since it was added to the cart.
            if (item.Book.Status != Book_Status.Published
                || item.Book.InStock <= 0)
            {
                throw new InvalidOperationException(
                    "This book is currently no longer available.");
            }

            // Ensures that the requested quantity does not exceed current stock.
            if (quantity > item.Book.InStock)
            {
                throw new InvalidOperationException(
                    $"Only {item.Book.InStock} unit available.");
            }

            // Updates the cart item's quantity with the validated value.
            item.Quantity = quantity;

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(
            string userId,
            int cartItemId)
        {
            // Retrieves the cart item only if it belongs to the specified user.
            // This prevents users from removing items from other users' carts.
            var item = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i =>
                    i.CartItemId == cartItemId &&
                    i.Cart.UserId == userId);
            // Throws an exception if the item does not exist
            // or does not belong to the user's cart.
            if (item == null)
            {
                throw new InvalidOperationException(
                    "The item does not exist in your cart.");
            }

            // Removes the selected item from the user's cart.
            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();
        }
    }
}
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

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();

                cart = await _context.Carts
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Book)
                    .FirstAsync(c => c.UserId == userId);
            }

            /*
            // If available stock decreased, adjust the quantity.
            bool changed = false;

            foreach (var item in cart.Items)
            {
                if (item.Book.Status == Book_Status.Published
                    && item.Book.InStock > 0
                    && item.Quantity > item.Book.InStock)
                {
                    item.Quantity = item.Book.InStock;
                    changed = true;
                }
            }

            if (changed)
            {
                await _context.SaveChangesAsync();
            }
            */
            return cart;
        }

        public async Task AddItemAsync(
            string userId,
            int bookId,
            int quantity)
        {
            if (quantity < 1 || quantity > 10)
            {
                throw new InvalidOperationException(
                    "La cantidad debe estar entre 1 y 10.");
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(b => b.BookId == bookId);

            if (book == null)
            {
                throw new InvalidOperationException(
                    "El libro no existe.");
            }

            if (book.Status != Book_Status.Published
                || book.InStock <= 0)
            {
                throw new InvalidOperationException(
                    "Este libro no está disponible actualmente.");
            }

            var cart = await GetOrCreateCartAsync(userId);

            var item = await _context.CartItems
                .FirstOrDefaultAsync(i =>
                    i.CartId == cart.CartId &&
                    i.BookId == bookId);

            int newQuantity = quantity;

            if (item != null)
            {
                newQuantity += item.Quantity;
            }

            if (newQuantity > 10)
            {
                throw new InvalidOperationException(
                    "No puedes agregar más de 10 unidades del mismo libro.");
            }

            if (newQuantity > book.InStock)
            {
                throw new InvalidOperationException(
                    $"Solo hay {book.InStock} unidades disponibles.");
            }

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

        public async Task UpdateQuantityAsync(
            string userId,
            int cartItemId,
            int quantity)
        {
            if (quantity < 1 || quantity > 10)
            {
                throw new InvalidOperationException(
                    "La cantidad debe estar entre 1 y 10.");
            }

            var item = await _context.CartItems
                .Include(i => i.Cart)
                .Include(i => i.Book)
                .FirstOrDefaultAsync(i =>
                    i.CartItemId == cartItemId &&
                    i.Cart.UserId == userId);

            if (item == null)
            {
                throw new InvalidOperationException(
                    "El artículo no existe en tu carrito.");
            }

            if (item.Book.Status != Book_Status.Published
                || item.Book.InStock <= 0)
            {
                throw new InvalidOperationException(
                    "Este libro ya no está disponible actualmente.");
            }

            if (quantity > item.Book.InStock)
            {
                throw new InvalidOperationException(
                    $"Solo hay {item.Book.InStock} unidades disponibles.");
            }

            item.Quantity = quantity;

            await _context.SaveChangesAsync();
        }

        public async Task RemoveItemAsync(
            string userId,
            int cartItemId)
        {
            var item = await _context.CartItems
                .Include(i => i.Cart)
                .FirstOrDefaultAsync(i =>
                    i.CartItemId == cartItemId &&
                    i.Cart.UserId == userId);

            if (item == null)
            {
                throw new InvalidOperationException(
                    "El artículo no existe en tu carrito.");
            }

            _context.CartItems.Remove(item);

            await _context.SaveChangesAsync();
        }
    }
}
using eBooks.Data.Enums;
using eBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Data.Services
{
    public class OrdersService : IOrdersService
    {
        private readonly AppDbContext _context;

        public OrdersService(AppDbContext context)
        {
            _context = context;
        }

        // Retrieve the order and verify that it belongs to the specified user.
        // Ensure that only pending orders can be cancelled.
        // Update the order status to Cancelled.
        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _context.Orders
            .FirstOrDefaultAsync(order =>
                order.OrderId == orderId &&
                order.UserId == userId);

                if (order == null)
                {
                    return false;
                }

                if (order.Status != Order_Status.Pending)
                {
                    return false;
                }

                order.Status = Order_Status.Cancelled;

                await _context.SaveChangesAsync();

                return true;
        }

        public async Task<int> CreateOrderAsync(
            string userId,
            int addressId)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. get user's address and validate it belongs to the user.
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a =>
                        a.AddressId == addressId &&
                        a.UserId == userId);

                if (address == null)
                {
                    throw new InvalidOperationException(
                        "The selected address does not exist.");
                }

                // 2. get cart user and validate books.
                var cart = await _context.Carts
                    .Include(c => c.Items)
                        .ThenInclude(i => i.Book)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null || !cart.Items.Any())
                {
                    throw new InvalidOperationException(
                        "Your cart is empty.");
                }

                // 3. Validate that all books are still published and have enough stock.
                foreach (var item in cart.Items)
                {
                    if (item.Book == null)
                    {
                        throw new InvalidOperationException(
                            "One of the books in the cart no longer exists..");
                    }

                    if (item.Book.Status != Book_Status.Published)
                    {
                        throw new InvalidOperationException(
                            $"Book '{item.Book.Title}' it is no longer published..");
                    }

                    if (item.Book.InStock < item.Quantity)
                    {
                        throw new InvalidOperationException(
                            $"Insufficient inventory for '{item.Book.Title}'. " +
                            $"Available: {item.Book.InStock}.");
                    }
                }

                // 4. build the order with the address snapshot.
                var order = new Order
                {
                    UserId = userId,
                    CreatedAtUtc = DateTime.UtcNow,
                    Status = Order_Status.Pending,

                    RecipientName = address.RecipientName,
                    Phone = address.Phone,
                    Street = address.Street,
                    Number = address.Number,
                    Neighborhood = address.Neighborhood,
                    City = address.City,
                    State = address.State,
                    PostalCode = address.PostalCode,
                    References = address.References
                };

                // 5. build the order items from the cart items.
                foreach (var item in cart.Items)
                {
                    order.Items.Add(new OrderItem
                    {
                        BookId = item.BookId,
                        BookTitle = item.Book.Title,
                        CoverURLPicture = item.Book.CoverURLPicture,
                        UnitPrice = item.Book.Price,
                        Quantity = item.Quantity
                    });
                }

                // 6. add the order to the database.
                _context.Orders.Add(order);

                // 7. delete the cart items after creating the order.
                _context.CartItems.RemoveRange(cart.Items);

                // 8. save and confirm the transaction.
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                // 9. return the order id.
                return order.OrderId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            // Retrieve all orders with their items and associated users, ordered by creation date.
            return await _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .Include(order => order.User)
            .OrderByDescending(order => order.CreatedAtUtc)
            .ToListAsync();
            }

        public async Task<Order?> GetOrderByIdAsync(
            int orderId,
            string userId)
        {
            // Retrieve the requested order and its items, ensuring it belongs to the specified user.
            return await _context.Orders
                .AsNoTracking()
                .Include(order => order.Items)
                .FirstOrDefaultAsync(order =>
                    order.OrderId == orderId &&
                    order.UserId == userId);
        }

        public async Task<Order?> GetOrderDetailsForAdminAsync(int orderId)
        {
            // Retrieve an order with its items and associated user for administrative review.
            return await _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .Include(order => order.User)
            .FirstOrDefaultAsync(order => order.OrderId == orderId);
            }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            // Retrieve all orders belonging to the specified user, ordered from newest to oldest.
            return await _context.Orders
            .AsNoTracking()
            .Include(order => order.Items)
            .Where(order => order.UserId == userId)
            .OrderByDescending(order => order.CreatedAtUtc)
            .ToListAsync();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, Order_Status newStatus)
        {
            // Retrieve the order to be updated.
            var order = await _context.Orders
        .FirstOrDefaultAsync(order => order.OrderId == orderId);

            // Return false if the requested order does not exist.
            if (order == null)
            {
                return false;
            }

            // Validate the requested status transition according to the order lifecycle rules.
            bool isValidTransition = order.Status switch
            {
                Order_Status.Pending =>
                    newStatus == Order_Status.Accepted ||
                    newStatus == Order_Status.Cancelled,

                Order_Status.Accepted =>
                    newStatus == Order_Status.Preparing ||
                    newStatus == Order_Status.Cancelled,

                Order_Status.Preparing =>
                    newStatus == Order_Status.Shipped ||
                    newStatus == Order_Status.Cancelled,

                Order_Status.Shipped =>
                    newStatus == Order_Status.Delivered,

                Order_Status.Delivered => false,

                Order_Status.Cancelled => false,

                _ => false
            };

            if (!isValidTransition)
            {
                return false;
            }

            // Apply the new status after validating the transition.
            order.Status = newStatus;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}

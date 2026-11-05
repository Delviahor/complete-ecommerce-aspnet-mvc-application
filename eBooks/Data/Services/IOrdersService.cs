using eBooks.Data.Enums;
using eBooks.Models;

namespace eBooks.Data.Services
{
    public interface IOrdersService
    {
        //Customer operations

        /// <summary>
        /// Creates a new order for the specified user using the selected delivery address.
        /// </summary>
        /// <param name="userId">The ID of the user placing the order.</param>
        /// <param name="addressId">The ID of the delivery address associated with the order.</param>
        /// <returns>The ID of the newly created order.</returns>
        Task<int> CreateOrderAsync(string userId, int addressId);

        /// <summary>
        /// Retrieves an order by its ID, ensuring it belongs to the specified user.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve.</param>
        /// <param name="userId">The ID of the user who owns the order.</param>
        /// <returns>The order if found and associated with the user; otherwise, null.</returns>
        Task<Order?> GetOrderByIdAsync(int orderId, string userId);

        /// <summary>
        /// Retrieves all orders placed by the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user whose orders are requested.</param>
        /// <returns>A list of the user's orders.</returns>
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);

        /// <summary>
        /// Cancels an order belonging to the specified user, if cancellation is allowed.
        /// </summary>
        /// <param name="orderId">The ID of the order to cancel.</param>
        /// <param name="userId">The ID of the user requesting the cancellation.</param>
        /// <returns>True if the order was successfully cancelled; otherwise, false.</returns>
        Task<bool> CancelOrderAsync(int orderId, string userId);

        //Admin operations

        /// <summary>
        /// Retrieves all orders placed by registered users.
        /// </summary>
        /// <returns>A list containing all orders.</returns>
        Task<List<Order>> GetAllOrdersAsync();

        /// <summary>
        /// Retrieves the complete details of an order for administrative purposes.
        /// </summary>
        /// <param name="orderId">The ID of the order to retrieve.</param>
        /// <returns>The order with its details if found; otherwise, null.</returns>
        Task<Order?> GetOrderDetailsForAdminAsync(int orderId);

        /// <summary>
        /// Updates the status of an existing order.
        /// </summary>
        /// <param name="orderId">The ID of the order to update.</param>
        /// <param name="newStatus">The new status to assign to the order.</param>
        /// <returns>True if the status was successfully updated; otherwise, false.</returns>
        Task<bool> UpdateOrderStatusAsync( int orderId, Order_Status newStatus);
    }
}
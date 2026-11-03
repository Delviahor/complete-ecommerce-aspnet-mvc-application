using eBooks.Data.Enums;
using eBooks.Models;

namespace eBooks.Data.Services
{
    public interface IOrdersService
    {
        //Customer operations
        Task<int> CreateOrderAsync(string userId, int addressId);
        Task<Order?> GetOrderByIdAsync(int orderId, string userId);
        Task<List<Order>> GetOrdersByUserIdAsync(string userId);
        Task<bool> CancelOrderAsync(int orderId, string userId);

        //Admin operations
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderDetailsForAdminAsync(int orderId);
        Task<bool> UpdateOrderStatusAsync( int orderId, Order_Status newStatus);
    }
}
using eBooks.Data.Enums;
using eBooks.Models;

namespace eBooks.Data.Services
{
    public interface IBookReviewsService
    {
        Task<List<BookReview>> GetPublishedByBookIdAsync(int bookId);

        Task<BookReview?> GetByUserAndBookAsync(
            string userId,
            int bookId);

        Task<BookReview> CreateAsync(
            string userId,
            int bookId,
            int rating,
            string content);

        Task UpdateAsync(
            int reviewId,
            string userId,
            int rating,
            string content);

        Task<List<BookReview>> GetPendingAsync();

        Task UpdateStatusAsync(
            int reviewId,
            Review_Status status);
    }
}
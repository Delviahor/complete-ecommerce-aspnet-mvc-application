using eBooks.Data.Enums;
using eBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Data.Services
{
    public class BookReviewsService : IBookReviewsService
    {
        private readonly AppDbContext _context;

        public BookReviewsService(AppDbContext context)
        {
            _context = context;
        }

        // Gets all publicly visible reviews for a book.
        public async Task<List<BookReview>> GetPublishedByBookIdAsync(
            int bookId)
        {
            return await _context.BookReviews
                .AsNoTracking()
                .Include(review => review.User)
                .Where(review =>
                    review.BookId == bookId &&
                    review.Status == Review_Status.Published)
                .OrderByDescending(review => review.CreatedAtUtc)
                .ToListAsync();
        }

        // Gets a user's review for a specific book.
        public async Task<BookReview?> GetByUserAndBookAsync(
            string userId,
            int bookId)
        {
            return await _context.BookReviews
                .AsNoTracking()
                .FirstOrDefaultAsync(review =>
                    review.UserId == userId &&
                    review.BookId == bookId);
        }

        // Creates a review and determines whether the purchase is verified.
        public async Task<BookReview> CreateAsync(
            string userId,
            int bookId,
            int rating,
            string content)
        {
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rating),
                    "Rating must be between 1 and 5.");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException(
                    "Review content cannot be empty.",
                    nameof(content));
            }

            var userExists = await _context.Users
                .AnyAsync(user => user.Id == userId);

            if (!userExists)
            {
                throw new InvalidOperationException(
                    "The specified user does not exist.");
            }

            var bookExists = await _context.Books
                .AnyAsync(book => book.BookId == bookId);

            if (!bookExists)
            {
                throw new InvalidOperationException(
                    "The specified book does not exist.");
            }

            var existingReview = await _context.BookReviews
                .AnyAsync(review =>
                    review.UserId == userId &&
                    review.BookId == bookId);

            if (existingReview)
            {
                throw new InvalidOperationException(
                    "You have already reviewed this book.");
            }

            // Only delivered orders count as verified purchases.
            var isVerifiedPurchase = await _context.OrderItems
                .AnyAsync(item =>
                    item.BookId == bookId &&
                    item.Order.UserId == userId &&
                    item.Order.Status == Order_Status.Delivered);

            var review = new BookReview
            {
                UserId = userId,
                BookId = bookId,
                Rating = rating,
                Content = content.Trim(),
                IsVerifiedPurchase = isVerifiedPurchase,
                Status = Review_Status.Pending,
                CreatedAtUtc = DateTime.UtcNow
            };

            _context.BookReviews.Add(review);

            await _context.SaveChangesAsync();

            return review;
        }

        // Updates a review owned by the specified user.
        public async Task UpdateAsync(
            int reviewId,
            string userId,
            int rating,
            string content)
        {
            if (rating < 1 || rating > 5)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(rating),
                    "Rating must be between 1 and 5.");
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException(
                    "Review content cannot be empty.",
                    nameof(content));
            }

            var review = await _context.BookReviews
                .FirstOrDefaultAsync(review =>
                    review.BookReviewId == reviewId &&
                    review.UserId == userId);

            if (review == null)
            {
                throw new InvalidOperationException(
                    "Review not found or you do not have permission to edit it.");
            }

            review.Rating = rating;
            review.Content = content.Trim();
            review.UpdatedAtUtc = DateTime.UtcNow;

            // Edited reviews return to moderation.
            review.Status = Review_Status.Pending;

            // Recalculate purchase verification.
            review.IsVerifiedPurchase = await _context.OrderItems
                .AnyAsync(item =>
                    item.BookId == review.BookId &&
                    item.Order.UserId == userId &&
                    item.Order.Status == Order_Status.Delivered);

            await _context.SaveChangesAsync();
        }

        // Gets reviews waiting for moderation.
        public async Task<List<BookReview>> GetPendingAsync()
        {
            return await _context.BookReviews
                .AsNoTracking()
                .Include(review => review.User)
                .Include(review => review.Book)
                .Where(review => review.Status == Review_Status.Pending)
                .OrderBy(review => review.CreatedAtUtc)
                .ToListAsync();
        }

        // Changes the moderation status of a review.
        public async Task UpdateStatusAsync(
            int reviewId,
            Review_Status status)
        {
            if (!Enum.IsDefined(typeof(Review_Status), status))
            {
                throw new ArgumentOutOfRangeException(nameof(status));
            }

            var review = await _context.BookReviews
                .FirstOrDefaultAsync(review =>
                    review.BookReviewId == reviewId);

            if (review == null)
            {
                throw new InvalidOperationException(
                    "Review not found.");
            }

            review.Status = status;

            await _context.SaveChangesAsync();
        }
    }
}
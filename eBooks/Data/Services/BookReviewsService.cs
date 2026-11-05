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

        // Retrieves all published reviews for a specific book.
        // Includes user information to display the reviewer's name and profile.
        // AsNoTracking improves query performance since these reviews
        // are only displayed and will not be modified.
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

        // Retrieves a specific user's review for a given book.
        // AsNoTracking is used because this operation only checks
        // whether a review exists and does not modify it.
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
            // Validates the review rating and content before processing.
            // Ratings must be between 1 and 5, and review content cannot be empty.
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
            // Ensures that the specified user exists before creating the review.
            var userExists = await _context.Users
                .AnyAsync(user => user.Id == userId);

            if (!userExists)
            {
                throw new InvalidOperationException(
                    "The specified user does not exist.");
            }
            // Ensures that the book exists before associating a review with it.
            var bookExists = await _context.Books
                .AnyAsync(book => book.BookId == bookId);

            if (!bookExists)
            {
                throw new InvalidOperationException(
                    "The specified book does not exist.");
            }
            // Prevents users from submitting more than one review
            // for the same book.
            var existingReview = await _context.BookReviews
                .AnyAsync(review =>
                    review.UserId == userId &&
                    review.BookId == bookId);

            if (existingReview)
            {
                throw new InvalidOperationException(
                    "You have already reviewed this book.");
            }

            // A purchase is considered verified only when the user
            // has a delivered order containing the reviewed book.
            // Pending, cancelled, or other order statuses do not qualify.
            var isVerifiedPurchase = await _context.OrderItems
                .AnyAsync(item =>
                    item.BookId == bookId &&
                    item.Order.UserId == userId &&
                    item.Order.Status == Order_Status.Delivered);

            // Creates the review with its initial moderation status.
            // All new reviews start as Pending and must be approved
            // before becoming publicly visible.
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
            // Validates the updated rating and content using the same rules
            // applied when creating a new review.
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

            // Retrieves the review only if it belongs to the specified user.
            // This prevents users from modifying reviews submitted by others.
            var review = await _context.BookReviews
                .FirstOrDefaultAsync(review =>
                    review.BookReviewId == reviewId &&
                    review.UserId == userId);

            // Throws an exception if the review does not exist
            // or the user does not own it.
            if (review == null)
            {
                throw new InvalidOperationException(
                    "Review not found or you do not have permission to edit it.");
            }
            // Updates the review content and records the modification timestamp.
            review.Rating = rating;
            review.Content = content.Trim();
            review.UpdatedAtUtc = DateTime.UtcNow;

            // Edited reviews return to Pending status.
            // This ensures that any changes are reviewed by an administrator
            // before becoming publicly visible again.
            review.Status = Review_Status.Pending;

            // Recalculates purchase verification using the user's current
            // delivered orders to keep the verification status accurate.
            review.IsVerifiedPurchase = await _context.OrderItems
                .AnyAsync(item =>
                    item.BookId == review.BookId &&
                    item.Order.UserId == userId &&
                    item.Order.Status == Order_Status.Delivered);

            await _context.SaveChangesAsync();
        }

        // Retrieves all reviews awaiting administrator moderation.
        // Includes user and book information to provide the administrator
        // with the context needed to evaluate each review.
        // Older submissions are displayed first.
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
            // Ensures that the provided moderation status is a valid
            // Review_Status enum value before updating the review.
            if (!Enum.IsDefined(typeof(Review_Status), status))
            {
                throw new ArgumentOutOfRangeException(nameof(status));
            }
            // Retrieves the review that will receive the new moderation status.
            var review = await _context.BookReviews
                .FirstOrDefaultAsync(review =>
                    review.BookReviewId == reviewId);
            // Throws an exception if the specified review does not exist.
            if (review == null)
            {
                throw new InvalidOperationException(
                    "Review not found.");
            }
            // Applies the administrator's moderation decision.
            // The new status determines whether the review is publicly visible.
            review.Status = status;

            await _context.SaveChangesAsync();
        }
    }
}
using eBooks.Data.Enums;
using eBooks.Models;

namespace eBooks.Data.Services
{
    /// <summary>
    /// Defines the operations available for managing book reviews,
    /// including creation, retrieval, updates, and moderation.
    /// </summary>
    public interface IBookReviewsService
    {
        /// <summary>
        /// Retrieves all published reviews for a specific book.
        /// </summary>
        /// <param name="bookId">The ID of the book.</param>
        /// <returns>A list of published reviews associated with the book.</returns>
        Task<List<BookReview>> GetPublishedByBookIdAsync(int bookId);

        // <summary>
        /// Retrieves a user's review for a specific book.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="bookId">The ID of the reviewed book.</param>
        /// <returns>The user's review if found; otherwise, null.</returns>
        Task<BookReview?> GetByUserAndBookAsync(
            string userId,
            int bookId);

        /// <summary>
        /// Creates a new review for a book.
        /// New reviews are submitted with Pending moderation status.
        /// Purchase verification is determined automatically.
        /// </summary>
        /// <param name="userId">The ID of the user submitting the review.</param>
        /// <param name="bookId">The ID of the reviewed book.</param>
        /// <param name="rating">The rating assigned to the book, from 1 to 5.</param>
        /// <param name="content">The written review content.</param>
        /// <returns>The newly created book review.</returns>
        Task<BookReview> CreateAsync(
            string userId,
            int bookId,
            int rating,
            string content);


        /// <summary>
        /// Updates an existing review belonging to the specified user.
        /// The updated review is returned to Pending moderation status.
        /// </summary>
        /// <param name="reviewId">The ID of the review to update.</param>
        /// <param name="userId">The ID of the review owner.</param>
        /// <param name="rating">The updated rating, from 1 to 5.</param>
        /// <param name="content">The updated review content.</param>
        Task UpdateAsync(
            int reviewId,
            string userId,
            int rating,
            string content);

        /// <summary>
        /// Retrieves all reviews awaiting administrator moderation.
        /// </summary>
        /// <returns>A list of pending reviews.</returns>
        Task<List<BookReview>> GetPendingAsync();

        /// <summary>
        /// Changes the moderation status of a specific review.
        /// </summary>
        /// <param name="reviewId">The ID of the review to moderate.</param>
        /// <param name="status">The new moderation status.</param>
        Task UpdateStatusAsync(
            int reviewId,
            Review_Status status);
    }
}
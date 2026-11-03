using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using eBooks.Data.Enums;

namespace eBooks.Models
{
    public class BookReview
    {
        [Key]
        public int BookReviewId { get; set; }

        // User who created the review.
        [Required]
        public required string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; } = null!;

        // Reviewed book.
        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; } = null!;

        // Rating from 1 to 5.
        [Range(1, 5)]
        public int Rating { get; set; }

        // Review content.
        [Required]
        [MaxLength(1500)]
        public required string Content { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }

        // Must be calculated by the server.
        public bool IsVerifiedPurchase { get; set; } = false;

        // Moderation status.
        public Review_Status Status { get; set; } = Review_Status.Pending;
    }
}

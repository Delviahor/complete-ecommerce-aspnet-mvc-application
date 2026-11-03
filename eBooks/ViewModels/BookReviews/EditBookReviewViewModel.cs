using System.ComponentModel.DataAnnotations;

namespace eBooks.ViewModels.BookReviews
{
    public class EditBookReviewViewModel
    {
        [Required]
        public int BookReviewId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required]
        [StringLength(1500, MinimumLength = 10)]
        public string Content { get; set; } = string.Empty;
    }
}
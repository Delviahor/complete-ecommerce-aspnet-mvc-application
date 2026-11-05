using eBooks.Models;

namespace eBooks.ViewModels.Catalog
{
    public class CatalogBookDetailsViewModel
    {
        public Book Book { get; set; } = null!;

        public List<BookReview> Reviews { get; set; } = new();

        // Calculates the average rating based on the available reviews.
        public double AverageRating =>
            Reviews.Count == 0
                ? 0
                : Reviews.Average(review => review.Rating);

        public int TotalReviews => Reviews.Count;

        public BookReview? CurrentUserReview { get; set; }
    }
}
using eBooks.Data.Enums;
using eBooks.Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBooks.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReviewModerationController : Controller
    {
        private readonly IBookReviewsService _bookReviewsService;

        public ReviewModerationController(
            IBookReviewsService bookReviewsService)
        {
            _bookReviewsService = bookReviewsService;
        }

        // Displays reviews waiting for moderation.
        public async Task<IActionResult> Index()
        {
            var reviews = await _bookReviewsService.GetPendingAsync();

            return View(reviews);
        }

        // Changes the moderation status of a review.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(
            int reviewId,
            Review_Status status)
        {
            try
            {
                await _bookReviewsService.UpdateStatusAsync(
                    reviewId,
                    status);

                TempData["Success"] =
                    "Review moderation status updated.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

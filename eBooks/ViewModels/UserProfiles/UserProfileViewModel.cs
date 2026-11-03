namespace eBooks.ViewModels.UserProfiles
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;

        public string? ProfilePicture { get; set; }

        public string? AboutMe { get; set; }

        public DateTime CreatedAtUtc { get; set; }

        public int PublishedReviewsCount { get; set; }
    }
}
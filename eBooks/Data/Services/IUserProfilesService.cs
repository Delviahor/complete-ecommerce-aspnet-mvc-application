using eBooks.Models;

namespace eBooks.Data.Services
{
    public interface IUserProfilesService
    {
        Task<UserProfile?> GetByUserIdAsync(string userId);

        Task<UserProfile> GetOrCreateAsync(string userId);

        Task UpdateAsync(
            string userId,
            string? displayName,
            string? aboutMe,
            string? profilePicture);

        Task<int> GetPublishedReviewsCountAsync(string userId);

    }

}

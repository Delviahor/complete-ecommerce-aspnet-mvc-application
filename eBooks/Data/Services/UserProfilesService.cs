using eBooks.Data.Enums;
using eBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Data.Services
{
    public class UserProfilesService : IUserProfilesService
    {
        private readonly AppDbContext _context;

        public UserProfilesService(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves a user profile by its associated Identity user ID.
        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.UserProfiles
            .AsNoTracking()
            .Include(profile => profile.User)
            .FirstOrDefaultAsync(profile => profile.UserId == userId); ;
        }

        // Retrieves an existing profile or creates one if it does not exist.
        public async Task<UserProfile> GetOrCreateAsync(string userId)
        {
            // Verify that the Identity user exists before creating a profile.
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(profile => profile.UserId == userId);

            if (profile != null)
            {
                return profile;
            }

            var userExists = await _context.Users
                .AnyAsync(user => user.Id == userId);

            if (!userExists)
            {
                throw new InvalidOperationException(
                    "The specified user does not exist.");
            }

            profile = new UserProfile
            {
                UserId = userId,
                CreatedAtUtc = DateTime.UtcNow
            };

            // Add the new profile and persist it to the database.
            _context.UserProfiles.Add(profile);

            await _context.SaveChangesAsync();

            return profile;
        }

        // Counts the published book reviews submitted by the specified user.
        public async Task<int> GetPublishedReviewsCountAsync(string userId)
        {
            return await _context.BookReviews
            .CountAsync(review =>
            review.UserId == userId &&
            review.Status == Review_Status.Published);
        }

        // Updates the profile's biography and profile picture.
        public async Task UpdateAsync(
            string userId,
            string? aboutMe,
            string? profilePicture)
        {
            var profile = await GetOrCreateAsync(userId);

            profile.AboutMe = aboutMe?.Trim();
            profile.ProfilePicture = profilePicture;
            profile.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // Updates the profile's display name, biography, and profile picture.
        public async Task UpdateAsync(
            string userId,
            string? displayName,
            string? aboutMe,
            string? profilePicture)
        {
            var profile = await GetOrCreateAsync(userId);

            profile.DisplayName = displayName?.Trim();
            profile.AboutMe = aboutMe?.Trim();
            profile.ProfilePicture = profilePicture;
            profile.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
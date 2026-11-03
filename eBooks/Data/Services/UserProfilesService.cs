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

        // Gets a profile by its Identity user ID.
        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.UserProfiles
            .AsNoTracking()
            .Include(profile => profile.User)
            .FirstOrDefaultAsync(profile => profile.UserId == userId); ;
        }

        // Gets an existing profile or creates a new one.
        public async Task<UserProfile> GetOrCreateAsync(string userId)
        {
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

            _context.UserProfiles.Add(profile);

            await _context.SaveChangesAsync();

            return profile;
        }

        public async Task<int> GetPublishedReviewsCountAsync(string userId)
        {
            return await _context.BookReviews
            .CountAsync(review =>
            review.UserId == userId &&
            review.Status == Review_Status.Published);
        }

        // Updates the public information of a profile.
        public async Task UpdateAsync(
            string userId,
            string? aboutMe,
            string? profilePicture)
        {
            var profile = await GetOrCreateAsync(userId);

            profile.AboutMe = aboutMe;
            profile.ProfilePicture = profilePicture;
            profile.UpdatedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}
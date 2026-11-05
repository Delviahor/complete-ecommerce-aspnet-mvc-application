using eBooks.Data.Services;
using eBooks.ViewModels.UserProfiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace eBooks.Controllers
{
    public class UserProfilesController : Controller
    {
        private readonly IUserProfilesService _userProfilesService;
        private readonly IProfilePictureService _profilePictureService;
        public UserProfilesController(
            IUserProfilesService userProfilesService,
            IProfilePictureService profilePictureService)
        {
            _userProfilesService = userProfilesService;
            _profilePictureService = profilePictureService;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return NotFound();

            var profile = await _userProfilesService.GetByUserIdAsync(id);

            // If the profile does not exist and the user visits his own profile,
            // we create your record.
            if (profile == null)
            {
                var currentUserId = User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

                if (currentUserId != id)
                    return NotFound();

                profile = await _userProfilesService.GetOrCreateAsync(id);

                // We perform another query to also load the information
                // for the user associated with the profile.
                profile = await _userProfilesService.GetByUserIdAsync(id);

                if (profile == null)
                    return NotFound();
            }

            var model = new UserProfileViewModel
            {
                UserId = profile.UserId,
                UserName = profile.User.UserName ?? "Unknown user",
                DisplayName = profile.DisplayName,
                ProfilePicture = profile.ProfilePicture,
                AboutMe = profile.AboutMe,
                CreatedAtUtc = profile.CreatedAtUtc,
                PublishedReviewsCount =
                    await _userProfilesService.GetPublishedReviewsCountAsync(id)
            };

            return View(model);
        }


        // Displays the form for editing one's own profile.
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var profile = await _userProfilesService
                .GetOrCreateAsync(userId);

            var model = new EditUserProfileViewModel
            {
                DisplayName = profile.DisplayName,
                AboutMe = profile.AboutMe,
                CurrentProfilePicture = profile.ProfilePicture
            };

            return View(model);
        }

        // Updates the user's own profile information.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            EditUserProfileViewModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Challenge();

            var profile = await _userProfilesService.GetOrCreateAsync(userId);

            if (!ModelState.IsValid)
            {
                model.CurrentProfilePicture = profile.ProfilePicture;
                return View(model);
            }

            string? newProfilePicture = null;

            try
            {
                // If an image was selected, we save the new one.
                if (model.ProfilePictureFile != null)
                {
                    newProfilePicture = await _profilePictureService
                        .SaveAsync(model.ProfilePictureFile);
                }

                // If no new image was uploaded, we keep the previous one.
                var finalProfilePicture =
                    newProfilePicture ?? profile.ProfilePicture;

                await _userProfilesService.UpdateAsync(
                    userId,
                    model.DisplayName,
                    model.AboutMe,
                    finalProfilePicture);

                // We delete the previous image only after
                // successfully updating the database.
                if (newProfilePicture != null &&
                    !string.IsNullOrEmpty(profile.ProfilePicture))
                {
                    await _profilePictureService.DeleteAsync(
                        profile.ProfilePicture);
                }

                TempData["Success"] = "Your profile has been updated.";

                return RedirectToAction(
                    nameof(Details),
                    new { id = userId });
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(
                    nameof(model.ProfilePictureFile),
                    ex.Message);
            }
            catch (IOException)
            {
                ModelState.AddModelError(
                    nameof(model.ProfilePictureFile),
                    "An error occurred while saving the image.");
            }

            // If the update fails, we delete the new file
            // to avoid leaving orphaned images.
            if (newProfilePicture != null)
            {
                await _profilePictureService.DeleteAsync(newProfilePicture);
            }

            model.CurrentProfilePicture = profile.ProfilePicture;

            return View(model);
        }
    }
}

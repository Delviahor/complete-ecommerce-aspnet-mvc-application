
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

            // Si el perfil no existe y el usuario visita su propio perfil,
            // creamos su registro.
            if (profile == null)
            {
                var currentUserId = User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

                if (currentUserId != id)
                    return NotFound();

                profile = await _userProfilesService.GetOrCreateAsync(id);

                // Volvemos a consultar para cargar también la información
                // del usuario asociado al perfil.
                profile = await _userProfilesService.GetByUserIdAsync(id);

                if (profile == null)
                    return NotFound();
            }

            var model = new UserProfileViewModel
            {
                UserId = profile.UserId,
                UserName = profile.User.UserName ?? "Unknown user",
                ProfilePicture = profile.ProfilePicture,
                AboutMe = profile.AboutMe,
                CreatedAtUtc = profile.CreatedAtUtc,
                PublishedReviewsCount =
                    await _userProfilesService.GetPublishedReviewsCountAsync(id)
            };

            return View(model);
        }


        // Muestra el formulario de edición del perfil propio.
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
                AboutMe = profile.AboutMe,
                CurrentProfilePicture = profile.ProfilePicture
            };

            return View(model);
        }

        // Actualiza la información del perfil propio.
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
                // Si se seleccionó una imagen, guardamos la nueva.
                if (model.ProfilePictureFile != null)
                {
                    newProfilePicture = await _profilePictureService
                        .SaveAsync(model.ProfilePictureFile);
                }

                // Si no se subió una imagen nueva, conservamos la anterior.
                var finalProfilePicture =
                    newProfilePicture ?? profile.ProfilePicture;

                await _userProfilesService.UpdateAsync(
                    userId,
                    model.AboutMe,
                    finalProfilePicture);

                // Eliminamos la imagen anterior solamente después
                // de actualizar correctamente la base de datos.
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

            // Si falla la actualización, eliminamos el archivo nuevo
            // para evitar dejar imágenes huérfanas.
            if (newProfilePicture != null)
            {
                await _profilePictureService.DeleteAsync(newProfilePicture);
            }

            model.CurrentProfilePicture = profile.ProfilePicture;

            return View(model);
        }
    }
}

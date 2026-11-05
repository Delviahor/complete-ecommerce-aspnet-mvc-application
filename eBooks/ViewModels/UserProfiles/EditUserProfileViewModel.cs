using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eBooks.ViewModels.UserProfiles
{
    public class EditUserProfileViewModel
    {
        [StringLength(50, ErrorMessage = "Display name cannot exceed 50 characters.")]
        public string? DisplayName { get; set; }

        [StringLength(500, ErrorMessage = "About Me cannot exceed 500 characters.")]
        public string? AboutMe { get; set; }

        public IFormFile? ProfilePictureFile { get; set; }

        public string? CurrentProfilePicture { get; set; }
    }
}
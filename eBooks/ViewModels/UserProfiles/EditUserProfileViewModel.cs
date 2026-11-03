using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace eBooks.ViewModels.UserProfiles
{
    public class EditUserProfileViewModel
    {
        [StringLength(500, ErrorMessage = "About Me cannot exceed 500 characters.")]
        public string? AboutMe { get; set; }

        public IFormFile? ProfilePictureFile { get; set; }

        public string? CurrentProfilePicture { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
namespace eBooks.ViewModels.Authors
{
    public class CreateAuthorViewModel
    {

        [Required(ErrorMessage = "The author name is mandatory..")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(600, ErrorMessage = "The biography cannot exceed 600 characters.")]
        public string? Biography { get; set; }

        public string? ProfileURLPicture { get; set; }
    }
}

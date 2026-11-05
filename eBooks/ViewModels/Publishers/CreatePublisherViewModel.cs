using System.ComponentModel.DataAnnotations;
namespace eBooks.ViewModels.Publishers
{
    public class CreatePublisherViewModel
    {
        [Required(ErrorMessage = "The name of the publisher is mandatory.")]
        [StringLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
        [StringLength(600, ErrorMessage = "The URL cannot exceed 600 characters.")]
        public string? ProfileURLPicture { get; set; }

    }
}

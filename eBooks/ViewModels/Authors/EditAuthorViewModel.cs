using System.ComponentModel.DataAnnotations;

namespace eBooks.ViewModels.Authors
{
    public class EditAuthorViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(600, ErrorMessage = "The biography cannot exceed 600 characters.")]
        public string? Biography { get; set; }
        public string? ProfileURLPicture { get; set; }
    }
}

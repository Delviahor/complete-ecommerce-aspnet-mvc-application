using System.ComponentModel.DataAnnotations;

namespace eBooks.ViewModels.Categories
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "The name is mandatory.")]
        [MaxLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}

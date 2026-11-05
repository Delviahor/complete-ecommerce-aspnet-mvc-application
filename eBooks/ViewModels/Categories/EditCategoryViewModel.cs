using eBooks.Models;
using System.ComponentModel.DataAnnotations;

namespace eBooks.ViewModels.Categories
{
    public class EditCategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "The name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;
        //public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}

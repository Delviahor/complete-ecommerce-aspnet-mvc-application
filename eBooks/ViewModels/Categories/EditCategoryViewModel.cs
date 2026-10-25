using eBooks.Models;
using System.ComponentModel.DataAnnotations;

namespace eBooks.ViewModels.Categories
{
    public class EditCategoryViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Name { get; set; } = string.Empty;
        //public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}

using System.ComponentModel.DataAnnotations;
namespace eBooks.ViewModels.Authors
{
    public class CreateAuthorViewModel
    {

        [Required(ErrorMessage = "El nombre del autor es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(600, ErrorMessage = "La biografia no puede superar los 600 caracteres.")]
        public string? Biography { get; set; }

        public string? ProfileURLPicture { get; set; }
    }
}

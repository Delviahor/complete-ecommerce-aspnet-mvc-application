using System.ComponentModel.DataAnnotations;
namespace eBooks.ViewModels.Publishers
{
    public class CreatePublisherViewModel
    {
        [Required(ErrorMessage = "El nombre de la editorial es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres.")]
        public string Name { get; set; }
        [StringLength(600, ErrorMessage = "La URL no puede superar los 600 caracteres.")]
        public string? ProfileURLPicture { get; set; }

    }
}

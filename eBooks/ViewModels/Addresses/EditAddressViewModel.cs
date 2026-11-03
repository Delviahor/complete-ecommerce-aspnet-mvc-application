
using System.ComponentModel.DataAnnotations;

namespace eBooks.ViewModels
{
    public class EditAddressViewModel
    {
        public int AddressId { get; set; }

        [Required(ErrorMessage = "El nombre del destinatario es obligatorio.")]
        [MaxLength(100)]
        public required string RecipientName { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [MaxLength(20)]
        public required string Phone { get; set; }

        [Required(ErrorMessage = "La calle es obligatoria.")]
        [MaxLength(150)]
        public required string Street { get; set; }

        [Required(ErrorMessage = "El número es obligatorio.")]
        [MaxLength(20)]
        public required string Number { get; set; }

        [Required(ErrorMessage = "La colonia es obligatoria.")]
        [MaxLength(100)]
        public required string Neighborhood { get; set; }

        [Required(ErrorMessage = "La ciudad es obligatoria.")]
        [MaxLength(100)]
        public required string City { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [MaxLength(100)]
        public required string State { get; set; }

        [Required(ErrorMessage = "El código postal es obligatorio.")]
        [MaxLength(10)]
        public required string PostalCode { get; set; }

        [MaxLength(300)]
        public string? References { get; set; }
    }
}

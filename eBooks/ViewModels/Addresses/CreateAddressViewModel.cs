
using System.ComponentModel.DataAnnotations;

namespace eBooks.ViewModels
{
    public class CreateAddressViewModel
    {
        [Required(ErrorMessage = "The recipient's name is required.")]
        [MaxLength(100)]
        public required string RecipientName { get; set; }

        [Required(ErrorMessage = "A phone number is required.")]
        [MaxLength(20)]
        public required string Phone { get; set; }

        [Required(ErrorMessage = "The street is mandatory.")]
        [MaxLength(150)]
        public required string Street { get; set; }

        [Required(ErrorMessage = "The number is mandatory.")]
        [MaxLength(20)]
        public required string Number { get; set; }

        [Required(ErrorMessage = "The neighborhood is mandatory")]
        [MaxLength(100)]
        public required string Neighborhood { get; set; }

        [Required(ErrorMessage = "The city is mandatory")]
        [MaxLength(100)]
        public required string City { get; set; }

        [Required(ErrorMessage = "The state is mandatory")]
        [MaxLength(100)]
        public required string State { get; set; }

        [Required(ErrorMessage = "The postal code is mandatory")]
        [MaxLength(10)]
        public required string PostalCode { get; set; }

        [MaxLength(300)]
        public string? References { get; set; }
    }
}

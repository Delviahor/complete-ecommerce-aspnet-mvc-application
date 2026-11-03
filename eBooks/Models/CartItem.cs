using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBooks.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        // Foreign Key: cart to which the items belongs
        public int CartId { get; set; }

        [ForeignKey(nameof(CartId))]
        public Cart Cart { get; set; } = null!;

        // Foreign Key: book that is being added to the cart
        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; } = null!;

        [Range(1, 10, ErrorMessage = "The quantity must be between 1 and 10.")]
        public int Quantity { get; set; } = 1;
    }
}

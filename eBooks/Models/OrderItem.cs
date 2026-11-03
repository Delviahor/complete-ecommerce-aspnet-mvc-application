
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBooks.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        // Foreign Key: order to which this item belongs
        public int OrderId { get; set; }

        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; } = null!;

        // optional reference to the book being purchased
        public int? BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book? Book { get; set; }

        // historic data of the book at the time of order creation
        [Required]
        [MaxLength(100)]
        public required string BookTitle { get; set; }

        [MaxLength(500)]
        public string? CoverURLPicture { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Range(1, 10)]
        public int Quantity { get; set; }
    }
}

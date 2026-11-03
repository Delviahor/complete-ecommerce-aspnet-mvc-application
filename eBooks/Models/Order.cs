
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using eBooks.Data.Enums;

namespace eBooks.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // Foreign Key: user who placed the order
        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; } = null!;

        // date of creation in UTC
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Estado independiente del estado de los libros
        public Order_Status Status { get; set; } = Order_Status.Pending;

        // Snapshot for the address at the time of order creation
        [Required]
        [MaxLength(100)]
        public required string RecipientName { get; set; }

        [Required]
        [MaxLength(20)]
        public required string Phone { get; set; }

        [Required]
        [MaxLength(150)]
        public required string Street { get; set; }

        [Required]
        [MaxLength(20)]
        public required string Number { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Neighborhood { get; set; }

        [Required]
        [MaxLength(100)]
        public required string City { get; set; }

        [Required]
        [MaxLength(100)]
        public required string State { get; set; }

        [Required]
        [MaxLength(10)]
        public required string PostalCode { get; set; }

        [MaxLength(300)]
        public string? References { get; set; }

        // items in the order
        public ICollection<OrderItem> Items { get; set; }
            = new List<OrderItem>();
    }
}

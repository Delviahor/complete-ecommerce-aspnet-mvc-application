using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace eBooks.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        // Foreign Key: user property to link the cart to a specific user
        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; } = null!;

        // Navigation Property
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
    }
}

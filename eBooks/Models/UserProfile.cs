using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace eBooks.Models
{
    public class UserProfile
    {
        // Identity user ID is also the primary key.
        [Key]
        public required string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; } = null!;

        // Public display name.
        [MaxLength(50)]
        public string? DisplayName { get; set; }

        // Profile image URL or stored file path.
        [MaxLength(500)]
        public string? ProfilePicture { get; set; }

        // Public user description.
        [MaxLength(500)]
        public string? AboutMe { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAtUtc { get; set; }
    }
}

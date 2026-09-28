using System.ComponentModel.DataAnnotations;

namespace eBooks.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        [MaxLength(100)]
        public required string Name { get; set; }
        
        public ICollection<Book>Books { get; set; } = new List<Book>();
    }
}

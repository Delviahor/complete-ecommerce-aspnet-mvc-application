using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBooks.Models
{
    public class Publisher
    {
        [Key]
        public int PublisherId { get; set; }
        
        [MaxLength(100)]
        public required string Name { get; set; }
        
        public string? ProfileURLPicture { get; set; }

        public bool IsVisible { get; set; } = true;

        //Navigation Properties
        public ICollection<Book> Books { get; set; } = new List<Book>();



    }
}

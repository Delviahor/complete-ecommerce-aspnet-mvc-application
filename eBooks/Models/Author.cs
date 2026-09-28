using System.ComponentModel.DataAnnotations;

namespace eBooks.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }
        public string? ProfileURLPicture { get; set; }
        [MaxLength(100)]
        public required string FullName { get; set; }
        public string? Biography { get; set; }

        //Navigation Properties
        public ICollection<Author_Book> AuthorBooks { get; set; } = new List<Author_Book>();

    }
}

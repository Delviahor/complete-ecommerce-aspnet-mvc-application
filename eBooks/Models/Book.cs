using eBooks.Data.Enums;
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eBooks.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }
        
        public int Pages { get; set; }
        public DateOnly PublicationDate { get; set; }

        public string? CoverURLPicture { get; set; }
        
        [MaxLength(100)]
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int InStock { get; set; }
        public decimal Price { get; set; }
        public Book_Language Language { get; set; }
        public Book_Status Status { get; set; }
        public Book_Format Format { get; set; }

        //Navigation Properties
        public ICollection<Author_Book> AuthorBooks { get; set; } = new List<Author_Book>();

        //Foregein Key for Category
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        //Foregein Key for Publisher
        public int PublisherId { get; set; }
        [ForeignKey(nameof(PublisherId))]
        public Publisher Publisher { get; set; } = null!;

    }
}

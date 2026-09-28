using eBooks.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eBooks.ViewModels.Books
{
    public class EditBookViewModel
    {
        public int BookId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }

        public string CoverURLPicture { get; set; } = string.Empty;


        //edit publisher, language, format, status, instock and pages

        public int PublisherId { get; set; }
        public IEnumerable<SelectListItem>? Publishers { get; set; }
        public Book_Language Language { get; set; }
        public Book_Format Format { get; set; }
        public Book_Status Status { get; set; }
        public int InStock { get; set; }
        public int Pages { get; set; }

        public List<int> AuthorIds { get; set; } = new();

        public IEnumerable<SelectListItem>? Authors { get; set; }
    }
}

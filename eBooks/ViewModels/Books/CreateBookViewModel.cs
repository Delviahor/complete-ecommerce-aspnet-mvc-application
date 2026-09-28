using eBooks.Data.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eBooks.ViewModels.Books
{
    public class CreateBookViewModel
    {
        public string Title { get; set; }

        public string? Description { get; set; }
        public int Pages { get; set; }
        public decimal Price { get; set; }
        public DateOnly PublicationDate { get; set; }
        public string? CoverURLPicture { get; set; }
        public int InStock { get; set; }
        public Book_Language Language { get; set; }
        public Book_Format Format { get; set; }
        //to select from the dropdown list
        public int CategoryId { get; set; }
        public int PublisherId { get; set; }
        public List<int> AuthorIds { get; set; } = [];
        public IEnumerable<SelectListItem>? Authors { get; set; } = [];
        public IEnumerable<SelectListItem> Categories { get; set; } = [];
        public IEnumerable<SelectListItem> Publishers { get; set; } = [];

    }
}

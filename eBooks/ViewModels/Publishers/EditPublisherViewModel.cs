using System.ComponentModel.DataAnnotations;

namespace eBooks.ViewModels.Publishers
{
    public class EditPublisherViewModel
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        
        public string? ProfileURLPicture { get; set; }
    }
}

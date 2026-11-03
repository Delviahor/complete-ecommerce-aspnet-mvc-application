
using System.ComponentModel.DataAnnotations;

namespace eBooks.ViewModels.Account
{
    public class AccountViewModel
    {
        public string? UserName { get; set; }

        [Display(Name = "Email address")]
        public string? Email { get; set; }

        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }
    }
}

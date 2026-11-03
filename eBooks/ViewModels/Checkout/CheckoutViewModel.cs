using System.ComponentModel.DataAnnotations;

namespace eBooks.ViewModels.Checkout
{
    public class CheckoutViewModel
    {
        public List<CheckoutItemViewModel> Items { get; set; }
            = new List<CheckoutItemViewModel>();

        public List<CheckoutAddressViewModel> Addresses { get; set; }
            = new List<CheckoutAddressViewModel>();

        [Required(ErrorMessage = "You must select a shipping address.")]
        public int? SelectedAddressId { get; set; }

        public decimal Total => Items.Sum(item => item.Subtotal);

        public bool HasUnavailableItems =>
            Items.Any(item => !item.IsAvailable);
    }
}
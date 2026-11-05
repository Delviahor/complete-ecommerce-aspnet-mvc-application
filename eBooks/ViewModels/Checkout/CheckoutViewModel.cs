using System.ComponentModel.DataAnnotations;

namespace eBooks.ViewModels.Checkout
{
    public class CheckoutViewModel
    {
        public List<CheckoutItemViewModel> Items { get; set; }
            = new List<CheckoutItemViewModel>();

        public List<CheckoutAddressViewModel> Addresses { get; set; }
            = new List<CheckoutAddressViewModel>();

        // The ID of the shipping address selected by the user.
        [Required(ErrorMessage = "You must select a shipping address.")]
        public int? SelectedAddressId { get; set; }

        // Calculates the total checkout amount based on item subtotals.
        public decimal Total => Items.Sum(item => item.Subtotal);

        // Indicates whether the cart contains unavailable items.
        public bool HasUnavailableItems =>
            Items.Any(item => !item.IsAvailable);
    }
}
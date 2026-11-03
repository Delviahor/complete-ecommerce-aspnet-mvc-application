namespace eBooks.ViewModels.Checkout
{
    public class CheckoutItemViewModel
    {
        public int BookId { get; set; }

        public required string BookTitle { get; set; }

        public string? CoverURLPicture { get; set; }

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal Subtotal => UnitPrice * Quantity;

        public int AvailableStock { get; set; }

        public bool IsAvailable { get; set; }
    }
}
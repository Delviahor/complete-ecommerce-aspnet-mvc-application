namespace eBooks.ViewModels.Checkout
{
    public class CheckoutAddressViewModel
    {
        public int AddressId { get; set; }

        public required string RecipientName { get; set; }

        public required string Phone { get; set; }

        public required string Street { get; set; }

        public required string Number { get; set; }

        public required string Neighborhood { get; set; }

        public required string City { get; set; }

        public required string State { get; set; }

        public required string PostalCode { get; set; }

        public string? References { get; set; }
    }
}
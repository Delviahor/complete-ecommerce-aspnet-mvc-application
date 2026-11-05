
using eBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Data.Services
{
    public class AddressesService : IAddressesService
    {
        private readonly AppDbContext _context;

        public AddressesService(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all addresses belonging to a specific user.
        // AsNoTracking improves read performance because the entities
        // will not be modified in this operation.
        public async Task<List<Address>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        // Retrieves an address only if it belongs to the specified user.
        // This prevents users from accessing other users' addresses.
        public async Task<Address?> GetByIdAsync(
            int addressId,
            string userId)
        {
            return await _context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.AddressId == addressId &&
                    a.UserId == userId);
        }

        public async Task AddAsync(Address address)
        {
            _context.Addresses.Add(address); // Adds a new address to the database and persists the changes.

            await _context.SaveChangesAsync();
        }

        // Finds the address using both its ID and the owner's user ID
        // to ensure that only the owner can update it.
        public async Task<bool> UpdateAsync(
            int addressId,
            string userId,
            Address updatedAddress)
        {
            var existingAddress = await _context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.AddressId == addressId &&
                    a.UserId == userId);

            // Returns false if the address does not exist
            // or does not belong to the specified user.
            if (existingAddress == null)
            {
                return false;
            }

            existingAddress.RecipientName = updatedAddress.RecipientName;
            existingAddress.Phone = updatedAddress.Phone;
            existingAddress.Street = updatedAddress.Street;
            existingAddress.Number = updatedAddress.Number;
            existingAddress.Neighborhood = updatedAddress.Neighborhood;
            existingAddress.City = updatedAddress.City;
            existingAddress.State = updatedAddress.State;
            existingAddress.PostalCode = updatedAddress.PostalCode;
            existingAddress.References = updatedAddress.References;

            await _context.SaveChangesAsync();

            return true;
        }

        // Verifies that the address belongs to the specified user
        // before allowing its deletion.
        public async Task<bool> DeleteAsync(
            int addressId,
            string userId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.AddressId == addressId &&
                    a.UserId == userId);

            if (address == null)
            {
                return false;
            }

            _context.Addresses.Remove(address);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}

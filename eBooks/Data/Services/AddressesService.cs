
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

        public async Task<List<Address>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Addresses
                .Where(a => a.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

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
            _context.Addresses.Add(address);

            await _context.SaveChangesAsync();
        }


        public async Task<bool> UpdateAsync(
            int addressId,
            string userId,
            Address updatedAddress)
        {
            var existingAddress = await _context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.AddressId == addressId &&
                    a.UserId == userId);

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

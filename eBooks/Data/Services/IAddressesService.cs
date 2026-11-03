using eBooks.Models;

namespace eBooks.Data.Services
{
    public interface IAddressesService
    {
        Task<List<Address>> GetAllByUserIdAsync(string userId);

        Task<Address?> GetByIdAsync(int addressId, string userId);

        Task AddAsync(Address address);

        Task<bool> UpdateAsync(
            int addressId,
            string userId,
            Address updatedAddress);

        Task<bool> DeleteAsync(int addressId, string userId);
    }
}

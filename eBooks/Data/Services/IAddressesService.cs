using eBooks.Models;

namespace eBooks.Data.Services
{
    /// <summary>
    /// Defines the operations available for managing user addresses.
    /// </summary>
    public interface IAddressesService
    {
        /// <summary>
        /// Retrieves all addresses belonging to a specific user.
        /// </summary>
        /// <param name="userId">The ID of the address owner.</param>
        /// <returns>A list of addresses associated with the user.</returns>
        Task<List<Address>> GetAllByUserIdAsync(string userId);

        /// <summary>
        /// Retrieves a specific address if it belongs to the specified user.
        /// </summary>
        /// <param name="addressId">The ID of the address.</param>
        /// <param name="userId">The ID of the address owner.</param>
        /// <returns>The address if found; otherwise, null.</returns>
        Task<Address?> GetByIdAsync(int addressId, string userId);

        /// <summary>
        /// Adds a new address to the database.
        /// </summary>
        /// <param name="address">The address information to be saved.</param>
        Task AddAsync(Address address);

        /// <summary>
        /// Updates an existing address belonging to the specified user.
        /// </summary>
        /// <param name="addressId">The ID of the address to update.</param>
        /// <param name="userId">The ID of the address owner.</param>
        /// <param name="updatedAddress">The updated address information.</param>
        /// <returns>True if the address was updated; otherwise, false.</returns>
        Task<bool> UpdateAsync(
            int addressId,
            string userId,
            Address updatedAddress);

        /// <summary>
        /// Deletes an address belonging to the specified user.
        /// </summary>
        /// <param name="addressId">The ID of the address to delete.</param>
        /// <param name="userId">The ID of the address owner.</param>
        /// <returns>True if the address was deleted; otherwise, false.</returns>
        Task<bool> DeleteAsync(int addressId, string userId);
    }
}

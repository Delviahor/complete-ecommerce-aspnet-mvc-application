using eBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Data.Services
{
    public class PublishersService : IPublishersService
    {
        private readonly AppDbContext _context;
        public PublishersService(AppDbContext context)
        {
            _context = context;
        }

        // Adds a new publisher to the database.
        public async Task AddAsync(Publisher publisher)
        {
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();
        }

        // Retrieves all publishers from the database.
        public async Task<IEnumerable<Publisher>> GetAllPublishersAsync()
        {
            return await _context.Publishers.ToListAsync();
        }

        // Retrieves a publisher by its ID, returning null if it does not exist.
        public async Task<Publisher?> GetByIdAsync(int id)
        {
            return await _context.Publishers.FirstOrDefaultAsync(a => a.PublisherId == id);
        }

        // Updates the information of an existing publisher.
        public async Task UpdateAsync(int id, Publisher publisher)
        {
            var existingPublisher = await _context.Publishers.FindAsync(id);

            // Ensure the publisher exists before applying the changes.
            if (existingPublisher == null)
            {
                throw new KeyNotFoundException(
                $"The publisher with ID {id} was not found.");
            }
            existingPublisher.Name = publisher.Name;
            existingPublisher.ProfileURLPicture = publisher.ProfileURLPicture;
            await _context.SaveChangesAsync();
        }
    }
}

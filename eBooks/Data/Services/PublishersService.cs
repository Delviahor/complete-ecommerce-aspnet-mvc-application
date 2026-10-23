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

        public async Task AddAsync(Publisher publisher)
        {
            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Publisher>> GetAllPublishersAsync()
        {
            return await _context.Publishers.ToListAsync();
        }

        public async Task<Publisher?> GetByIdAsync(int id)
        {
            return await _context.Publishers.FirstOrDefaultAsync(a => a.PublisherId == id);
        }

        public async Task UpdateAsync(int id, Publisher publisher)
        {
            var existingPublisher = await _context.Publishers.FindAsync(id);
            if (existingPublisher == null)
            {
                throw new KeyNotFoundException(
                $"No se encontró la editorial con ID {id}.");
            }
            existingPublisher.Name = publisher.Name;
            existingPublisher.ProfileURLPicture = publisher.ProfileURLPicture;
            await _context.SaveChangesAsync();
        }
    }
}

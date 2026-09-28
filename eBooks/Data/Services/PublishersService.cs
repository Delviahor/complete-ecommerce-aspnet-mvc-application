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
        public async Task<IEnumerable<Publisher>> GetAllPublishersAsync()
        {
            return await _context.Publishers.ToListAsync();
        }
    }
}

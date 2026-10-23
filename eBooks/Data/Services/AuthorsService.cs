using eBooks.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace eBooks.Data.Services
{
    public class AuthorsService : IAuthorsService
    {
        private readonly AppDbContext _context;

        public AuthorsService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        public async Task AddAsync(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
        }
        public async Task<Author?> GetByIdAsync(int id)
        {
            return await _context.Authors.FirstOrDefaultAsync(a => a.AuthorId == id);
        }
        public async Task UpdateAsync(int id, Author author)
        {
            var existingAuthor = await _context.Authors.FindAsync(id);
            if (existingAuthor == null)
            {
                throw new KeyNotFoundException(
                $"No se encontró el autor con ID {id}.");
            }
            existingAuthor.FullName = author.FullName;
            existingAuthor.ProfileURLPicture = author.ProfileURLPicture;
            existingAuthor.Biography = author.Biography;
            await _context.SaveChangesAsync();
        }
    }
}

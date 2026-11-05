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
        // Retrieves all authors from the database.
        public async Task<IEnumerable<Author>> GetAllAuthorsAsync()
        {
            return await _context.Authors.ToListAsync();
        }

        // Adds a new author and saves the changes to the database.
        public async Task AddAsync(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
        }
        // Retrieves an author by its ID.
        // Returns null if no matching author is found.
        public async Task<Author?> GetByIdAsync(int id)
        {
            return await _context.Authors.FirstOrDefaultAsync(a => a.AuthorId == id);
        }
        public async Task UpdateAsync(int id, Author author)
        {
            // Finds the existing author that will be updated.
            var existingAuthor = await _context.Authors.FindAsync(id);

            // Throws an exception if the author does not exist.
            if (existingAuthor == null)
            {
                throw new KeyNotFoundException(
                $"Author with ID {id} not found.");
            }
            // Updates the author's information with the provided values.
            existingAuthor.FullName = author.FullName;
            existingAuthor.ProfileURLPicture = author.ProfileURLPicture;
            existingAuthor.Biography = author.Biography;
            await _context.SaveChangesAsync();
        }
    }
}

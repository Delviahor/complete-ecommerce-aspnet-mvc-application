using eBooks.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace eBooks.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Author_Book>().HasKey(ab => new
            {
                ab.AuthorId,
                ab.BookId
            });

            modelBuilder.Entity<Author_Book>()
                .HasOne(authorBook => authorBook.Book)
                .WithMany(book => book.AuthorBooks)
                .HasForeignKey(ab => ab.BookId);

            modelBuilder.Entity<Author_Book>()
                .HasOne(authorBook => authorBook.Author)
                .WithMany(author => author.AuthorBooks)
                .HasForeignKey(ab => ab.AuthorId);

            modelBuilder.Entity<Book>()
                .Property(book => book.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Book>()
                .HasOne(book => book.Category)
                .WithMany(category => category.Books)
                .HasForeignKey(book => book.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Book>()
                .HasOne(book => book.Publisher)
                .WithMany(publisher => publisher.Books)
                .HasForeignKey(book => book.PublisherId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author_Book> Author_Books { get; set; }
    }
}

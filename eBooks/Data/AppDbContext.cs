using eBooks.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using eBooks.Data.Enums;

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
            //
            // UserProfile: one profile per IdentityUser
            modelBuilder.Entity<UserProfile>()
                .HasOne(profile => profile.User)
                .WithOne()
                .HasForeignKey<UserProfile>(profile => profile.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // BookReview: one user can write multiple reviews
            modelBuilder.Entity<BookReview>()
                .HasOne(review => review.User)
                .WithMany()
                .HasForeignKey(review => review.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // BookReview: one book can have multiple reviews
            modelBuilder.Entity<BookReview>()
                .HasOne(review => review.Book)
                .WithMany()
                .HasForeignKey(review => review.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // BookReview: one review per user per book
            modelBuilder.Entity<BookReview>()
                .HasIndex(review => new { review.UserId, review.BookId })
                .IsUnique();

            // BookReview: default moderation status
            modelBuilder.Entity<BookReview>()
                .Property(review => review.Status)
                .HasDefaultValue(Review_Status.Pending);
            //
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

            // Cart: one cart per user
            modelBuilder.Entity<Cart>()
                .HasIndex(cart => cart.UserId)
                .IsUnique();

            // CartItem: one entry per book in each cart
            modelBuilder.Entity<CartItem>()
                .HasIndex(item => new { item.CartId, item.BookId })
                .IsUnique();

            // Cart -> IdentityUser
            modelBuilder.Entity<Cart>()
                .HasOne(cart => cart.User)
                .WithOne()
                .HasForeignKey<Cart>(cart => cart.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cart -> CartItem
            modelBuilder.Entity<CartItem>()
                .HasOne(item => item.Cart)
                .WithMany(cart => cart.Items)
                .HasForeignKey(item => item.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem -> Book
            modelBuilder.Entity<CartItem>()
                .HasOne(item => item.Book)
                .WithMany()
                .HasForeignKey(item => item.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // Address -> IdentityUser
            modelBuilder.Entity<Address>()
                .HasOne(address => address.User)
                .WithMany()
                .HasForeignKey(address => address.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order -> IdentityUser
            modelBuilder.Entity<Order>()
                .HasOne(order => order.User)
                .WithMany()
                .HasForeignKey(order => order.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            // Order -> OrderItem
            modelBuilder.Entity<OrderItem>()
                .HasOne(item => item.Order)
                .WithMany(order => order.Items)
                .HasForeignKey(item => item.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            // OrderItem -> BookId (optional)
            modelBuilder.Entity<OrderItem>()
                .HasOne(item => item.Book)
                .WithMany()
                .HasForeignKey(item => item.BookId)
                .OnDelete(DeleteBehavior.SetNull);
            // OrderItem -> UnitPrice precision
            modelBuilder.Entity<OrderItem>()
                .Property(item => item.UnitPrice)
                .HasPrecision(18, 2);
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author_Book> Author_Books { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<BookReview> BookReviews { get; set; }
    }
}

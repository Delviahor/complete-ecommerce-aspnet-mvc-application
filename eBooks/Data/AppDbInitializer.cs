using eBooks.Data.Enums;
using eBooks.Models;

namespace eBooks.Data
{
    public class AppDbInitializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();

                context.Database.EnsureCreated();

                // Seed Categories
                if (!context.Categories.Any())
                {
                    context.Categories.AddRange(new List<Category>()
                    {
                        new Category() { Name = "DRAFT" },
                        new Category() { Name = "Science Fiction" },
                        new Category() { Name = "Fantasy" },
                        new Category() { Name = "Mystery" },
                        new Category() { Name = "Romance" },
                        new Category() { Name = "Horror" }
                    });
                    context.SaveChanges();
                }

                // Seed Publishers
                if (!context.Publishers.Any())
                {
                    context.Publishers.AddRange(new List<Publisher>()
                    {
                        new Publisher() { Name = "Penguin Random House" },
                        new Publisher() { Name = "HarperCollins" },
                        new Publisher() { Name = "Simon & Schuster" },
                        new Publisher() { Name = "Hachette Book Group" },
                        new Publisher() { Name = "Macmillan Publishers" }
                    });
                    context.SaveChanges();
                }
                // Seed Authors
                if (!context.Authors.Any())
                {
                    context.Authors.AddRange(new List<Author>()
                    {
                        new Author() { FullName = "Isaac Asimov", Biography = "American writer and biochemist best known for his science fiction novels and popular science books. Author of the Foundation and Robot series." },
                        new Author() { FullName = "J.K. Rowling", Biography = "British author best known for creating the Harry Potter series, one of the most successful fantasy franchises in history." },
                        new Author() { FullName = "Agatha Christie", Biography = "British writer widely regarded as the Queen of Mystery. Creator of the famous detectives Hercule Poirot and Miss Marple." },
                        new Author() { FullName = "Stephen King", Biography = "American author renowned for his horror, suspense, and supernatural fiction. Writer of classics such as The Shining, It, and Misery." },
                        new Author() { FullName = "Jane Austen", Biography = "English novelist celebrated for her timeless works on society and relationships, including Pride and Prejudice and Emma." }
                    });
                    context.SaveChanges();
                }

                // Seed Books
                if (!context.Books.Any())
                {
                    context.Books.AddRange(
                    new Book
                    {
                        Title = "Foundation",
                        Pages = 255,
                        PublicationDate = new DateOnly(1951, 6, 1),
                        Description = "A science fiction novel about the fall of a Galactic Empire and the effort to preserve knowledge through the Foundation.",
                        InStock = 20,
                        Price = 19.99m,
                        Language = Book_Language.English,
                        Status = Book_Status.Published,
                        Format = Book_Format.Paperback,
                        CategoryId = 1,
                        PublisherId = 1
                    });
                    context.SaveChanges();
                }

                //Seed Authors & Books
                if (!context.Author_Books.Any())
                {
                    context.Author_Books.AddRange(
                    new Author_Book
                    {
                        AuthorId = 1, // Isaac Asimov
                        BookId = 1  // Foundation
                    });
                    context.SaveChanges();
                }
            }
        }
    }
}

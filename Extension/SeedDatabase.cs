using Bogus;
using Bogus.DataSets;
using Library.Data;
using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Extension
{
    public static class SeedDatabase
    {
        private static Faker _faker = new Faker();

        public static void SeedData(this IServiceCollection _, IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<DbContextOptions<LibraryDbContext>>();
            using var context = new LibraryDbContext(options);

            CreateCategory(context);
            CreateAuthors(context);
            CreateBooks(context);
            CreatePublication(context);
        }

        private static void CreateCategory(LibraryDbContext context)
        {
            if (context.Categories.Any()) return;

            List<string> categoryNames = new();
            List<Category> categories = new();

            for (int i = 0; i < 10; i++)
            {
                var categoryName = _faker.Commerce
                    .Categories(1)
                    .First();

                int attempts = 0;

                while (categoryNames.Contains(categoryName) && attempts < 100)
                {
                    categoryName = _faker.Commerce
                        .Categories(1)
                        .First();
                    attempts++;
                }

                categoryNames.Add(categoryName);
                categories.Add(new Category
                {
                    Name = categoryName,
                });
            }

            context.AddRange(categories);
            context.SaveChanges();
        }

        private static void CreateAuthors(LibraryDbContext context)
        {
            if (context.Authors.Any()) return;

            List<Author> authors = new List<Author>();

            for (int i = 0; i < 100; i++)
            {
                authors.Add(new Author()
                {
                    FullName = _faker.Name.FullName(),
                    BirthDate = _faker.Person.DateOfBirth,
                    Email = _faker.Person.Email,
                    PhoneNumber = _faker.Phone.PhoneNumber("+998-##-###-##-##")
                });
            }

            context.Authors.AddRange(authors);
            context.SaveChanges();
        }

        private static void CreateBooks(LibraryDbContext context)
        {
            if (context.Books.Any()) return;

            List<Category> categoirs = context.Categories.ToList();
            List<Book> books = new List<Book>();

            foreach(Category category in categoirs)
            {
                for(int i = 0; i <= 50; i++)
                {
                    books.Add(new Book()
                    {
                        Name = _faker.Lorem.Word(),
                        Description = _faker.Lorem.Sentences(),
                        Price = _faker.Random.Decimal(30_000,300_000),
                        CategoryId = category.Id
                    });
                }
            }

            context.Books.AddRange(books);
            context.SaveChanges();
        }

        private static void CreatePublication(LibraryDbContext context)
        {
            if(context.Publications.Any()) return;

            List<Author> authors = context.Authors.ToList();
            List<Book> books = context.Books.ToList();

            List<Publication> publications = new List<Publication>();

            foreach(Book book in books)
            {
                var randomAuthor = _faker.PickRandom(authors);

                publications.Add(new Publication()
                {
                    BookId = book.Id,
                    AuthorId = randomAuthor.Id,
                    PublishedDate = _faker.Date.Between(DateTime.Now.AddYears(-18),DateTime.Now),
                });
            }

            context.Publications.AddRange(publications);
            context.SaveChanges();
        }
    }
}

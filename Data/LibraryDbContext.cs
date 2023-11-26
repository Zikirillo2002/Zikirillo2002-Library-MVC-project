using Library.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Data
{
    public class LibraryDbContext : DbContext
    {
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Author> Authors { get; set; }
        public virtual DbSet<Publication> Publications { get; set; }

        public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}

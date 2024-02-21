using BookLibrary.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Server.Database;

public class LibraryDbContext(DbContextOptions<LibraryDbContext> options) : DbContext(options)
{
    public DbSet<Book> Books { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Author> Authors { get; set; }

    public DbSet<AdminUser> AdminUsers { get; set; }
}
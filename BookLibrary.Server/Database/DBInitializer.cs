using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using BookLibrary.Server.Models;
using BookLibrary.Server.Services;

namespace BookLibrary.Server.Database;

public class DbInitializer
{
    public static void Initialize(LibraryDbContext bookDbContext)
    {
        if (bookDbContext.Books.Any())
            return;

        var genres = new List<Genre>
        {
            new() { Name = "Fantasy" },
            new() { Name = "Science Fiction" },
            new() { Name = "Horror" },
            new() { Name = "Romance" }
        };
        bookDbContext.AddRange(genres);

        var authors = new List<Author>
        {
            new() { FirstName = "J.R.R.", LastName = "Tolkien" },
            new() { FirstName = "Isaac", LastName = "Asimov" },
            new() { FirstName = "Stephen", LastName = "King" },
            new() { FirstName = "Jane", LastName = "Austen" }
        };
        bookDbContext.AddRange(authors);
        bookDbContext.SaveChanges();

        var books = new List<Book>
        {
            new()
            {
                Name = "The Hobbit", Authors = new List<Author> { authors[0] }, Price = 10.99m,
                Genres = new List<Genre> { genres[0] }, Description = GenerateDummyContent(Random.Shared.Next(50, 100)),
                Pages = new List<string> { GenerateDummyContent(Random.Shared.Next(100, 200)), GenerateDummyContent(Random.Shared.Next(100, 200)), GenerateDummyContent(Random.Shared.Next(100, 200)) }
            },
            new()
            {
                Name = "The Lord of the Rings", Authors = new List<Author> { authors[1] }, Price = 19.99m,
                Genres = new List<Genre> { genres[0] }, Description = GenerateDummyContent(Random.Shared.Next(50, 100)),
                Pages = new List<string> { GenerateDummyContent(Random.Shared.Next(100, 200)), GenerateDummyContent(Random.Shared.Next(100, 200)), GenerateDummyContent(Random.Shared.Next(100, 200)) }
            },
            new()
            {
                Name = "Foundation", Authors = new List<Author> { authors[0], authors[2] }, Price = 14.99m,
                Genres = new List<Genre> { genres[1] }, Description = GenerateDummyContent(Random.Shared.Next(50, 100)),
                Pages = new List<string> { GenerateDummyContent(Random.Shared.Next(100, 200)), GenerateDummyContent(Random.Shared.Next(100, 200)), GenerateDummyContent(Random.Shared.Next(100, 200)) }
            },
            new()
            {
                Name = "It", Authors = new List<Author> { authors[3] }, Price = 12.99m,
                Genres = new List<Genre> { genres[2] }, Description = GenerateDummyContent(Random.Shared.Next(50, 100)),
                Pages = new List<string> { GenerateDummyContent(Random.Shared.Next(100, 200)), GenerateDummyContent(Random.Shared.Next(100, 200)), GenerateDummyContent(Random.Shared.Next(100, 200)) }
            },
            new()
            {
                Name = "Pride and Prejudice", Authors = new List<Author> { authors[2], authors[3] }, Price = 9.99m,
                Genres = new List<Genre> { genres[3] }, Description = GenerateDummyContent(Random.Shared.Next(50, 100)),
                Pages = new List<string> { GenerateDummyContent(Random.Shared.Next(100, 200)), GenerateDummyContent(Random.Shared.Next(100, 200)), GenerateDummyContent(Random.Shared.Next(100, 200)) }
            }
        };
        bookDbContext.AddRange(books);

        var adminUser = new AdminUser
        {
            UserName = "Admin", 
            PasswordHash = AuthenticationService.HashPassword("mbds"), 
            Roles = AdminRole.All,
            AvatarUrl = "//m.gettywallpapers.com/wp-content/uploads/2023/09/Anya-Dp.jpg"
        };
        bookDbContext.Add(adminUser);

        bookDbContext.SaveChanges();
    }

    private static string GenerateDummyContent(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        Span<byte> buffer = stackalloc byte[length];
        RandomNumberGenerator.Fill(buffer);
        Span<char> span = stackalloc char[length];
        for (var i = 0; i < length; i++)
            span[i] = chars[buffer[i] % chars.Length];

        return new string(span);
    }
}
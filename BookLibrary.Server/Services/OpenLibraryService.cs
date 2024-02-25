using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookLibrary.Server.Models;
using OpenLibraryNET;

namespace BookLibrary.Server.Services;

public class OpenLibraryService
{
    private readonly OpenLibraryClient _client = new OpenLibraryClient();
    
    public async Task<Book> GetBookAsync(string isbn)
    {
        try
        {
            var book = await _client.GetWorkAsync(isbn);
            if (book.Data is null)
                return null;

            var authors = new List<OLAuthor>();
            foreach (var authorKey in book.Data.AuthorKeys)
            {
                var author = await _client.GetAuthorAsync(authorKey);
                if (author.Data is null)
                    continue;
                
                authors.Add(author);
            }
        
            var bookModel = new Book
            {
                Name = book.Data.Title,
                Content = book.Data.Description,
                Price = 0.0m,
                Authors = new List<Author>(),
                Genres = new List<Genre>()
            };
            foreach (var author in authors)
            {
                int lastSpace = author.Data!.Name.LastIndexOf(' ');
                if (lastSpace == -1)
                    continue;
                
                bookModel.Authors.Add(new Author
                {
                    FirstName = author.Data.Name[..lastSpace],
                    LastName = author.Data.Name[(lastSpace + 1)..]
                });
            }
            
            return bookModel;
        }
        catch (Exception e)
        {
            return null;
        }

    }
}
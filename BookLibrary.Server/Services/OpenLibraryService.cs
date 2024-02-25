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
                Description = book.Data.Description,
                Price = 0.0m,
                Authors = new List<Author>(),
                Genres = new List<Genre>()
            };
            foreach (var author in authors)
            {
                if (ExtractAuthorName(author.Data!.Name, out var firstName, out var lastName))
                {
                    bookModel.Authors.Add(new Author
                    {
                        FirstName = firstName,
                        LastName = lastName
                    });
                }
            }
            
            return bookModel;
        }
        catch (Exception e)
        {
            return null;
        }

    }
    
    private static bool ExtractAuthorName(string authorName, out string firstName, out string lastName)
    {
        firstName = string.Empty;
        lastName = string.Empty;
        
        ReadOnlySpan<char> authorNameSpan = authorName;
        int separatorIndex = authorNameSpan.IndexOf('-');
        if (separatorIndex != -1)
            authorNameSpan = authorNameSpan[..separatorIndex].Trim();
        
        int lastSpace = authorNameSpan.LastIndexOf(' ');
        if (lastSpace == -1)
            return false;
        
        firstName = authorNameSpan[..lastSpace].ToString();
        lastName = authorNameSpan[(lastSpace + 1)..].ToString();
        return true;
    }
}
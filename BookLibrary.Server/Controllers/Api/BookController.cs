using System;
using System.Linq;
using System.Threading.Tasks;
using BookLibrary.Server.Database;
using BookLibrary.Server.Dtos;
using BookLibrary.Server.Models;
using BookLibrary.Server.Models.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Server.Controllers.Api;

[Route("/api/books")]
public class BookController(LibraryDbContext libraryDbContext, DtoMapper mapper) : ApiController
{
    [HttpGet]
    public async Task<GetBooksResponse> GetBooks([FromQuery] GetBooksRequest request)
    {
        IQueryable<Book> bookQuery = libraryDbContext.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres);
        if (request.Genre is {Count:>0})
            bookQuery = bookQuery.Where(b => b.Genres.Any(g => request.Genre.Contains(g.Id)));
        if (request.Author is {Count:>0})
            bookQuery = bookQuery.Where(b => b.Authors.Any(g => request.Author.Contains(g.Id)));
        if (request.Offset > 0)
            bookQuery = bookQuery.Skip(request.Offset);
        
        bookQuery = bookQuery.Take(request.Limit + 1);
        
        var bookResponse = mapper.ToBookListEntryDto(await bookQuery.ToListAsync());
        return new GetBooksResponse
        {
            Data = bookResponse.Length > request.Limit 
                ? new ArraySegment<BookListEntryDto>(bookResponse, 0, request.Limit) 
                : new ArraySegment<BookListEntryDto>(bookResponse),
            HasMore = bookResponse.Length > request.Limit
        };
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        Book book = await libraryDbContext.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (book is null)
            return NotFound();
        return mapper.ToBookDto(book);
    }
}
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

[Route("/api/authors")]
public class AuthorController(LibraryDbContext libraryDbContext, DtoMapper mapper) : ApiController
{
    [HttpGet]
    public async Task<GetAuthorsResponse> GetAuthors([FromQuery] GetAuthorsRequest request)
    {
        IQueryable<Author> authorQuery = libraryDbContext.Authors;
        
        if (!string.IsNullOrWhiteSpace(request.Query)) // concatenate first and last name and search for the query (case-insensitive)
            authorQuery = authorQuery.Where(a => EF.Functions.Like($"{a.FirstName} {a.LastName}", $"%{request.Query}%"));
        if (request.Offset > 0)
            authorQuery = authorQuery.Skip(request.Offset);
        
        authorQuery = authorQuery.Take(request.Limit + 1);
        
        var authorResponse = mapper.ToAuthorDto(await authorQuery.ToListAsync());
        return new GetAuthorsResponse
        {
            Data = authorResponse.Length > request.Limit 
                ? new ArraySegment<AuthorDto>(authorResponse, 0, request.Limit) 
                : new ArraySegment<AuthorDto>(authorResponse),
            HasMore = authorResponse.Length > request.Limit
        };
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
    {
        Author author = await libraryDbContext.Authors.FirstOrDefaultAsync(b => b.Id == id);
        if (author is null)
            return NotFound();
        return mapper.ToAuthorDto(author);
    }
}
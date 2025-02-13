﻿using System;
using System.Linq;
using System.Threading.Tasks;
using BookLibrary.Server.Database;
using BookLibrary.Server.Dtos;
using BookLibrary.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Server.Controllers.Api;

[Route("/api/genres")]
public class GenreController(LibraryDbContext libraryDbContext, DtoMapper mapper) : ApiController
{
    [HttpGet]
    public async Task<GetGenresResponse> GetGenres([FromQuery] GetGenresRequest request)
    {
        IQueryable<Genre> genreQuery = libraryDbContext.Genres;

        if (!string.IsNullOrWhiteSpace(request.Query))
            genreQuery = genreQuery.Where(g => EF.Functions.Like(g.Name, $"%{request.Query}%")); // case-insensitive
        if (request.Offset > 0)
            genreQuery = genreQuery.Skip(request.Offset);

        genreQuery = genreQuery.Take(request.Limit + 1);

        var genreResponse = mapper.ToGenreDto(await genreQuery.ToListAsync());
        return new GetGenresResponse
        {
            Data = genreResponse.Length > request.Limit
                ? new ArraySegment<GenreDto>(genreResponse, 0, request.Limit)
                : new ArraySegment<GenreDto>(genreResponse),
            HasMore = genreResponse.Length > request.Limit
        };
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GenreDto>> GetGenre(int id)
    {
        var genre = await libraryDbContext.Genres.FirstOrDefaultAsync(b => b.Id == id);
        if (genre is null)
            return NotFound();
        return mapper.ToGenreDto(genre);
    }
}
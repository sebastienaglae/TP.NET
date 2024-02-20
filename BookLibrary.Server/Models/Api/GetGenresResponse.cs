using System;
using System.Linq;
using BookLibrary.Server.Dtos;

namespace BookLibrary.Server.Models.Api;

public class GetGenresResponse
{
    public ArraySegment<GenreDto> Data { get; set; }
    public bool HasMore { get; set; }
}
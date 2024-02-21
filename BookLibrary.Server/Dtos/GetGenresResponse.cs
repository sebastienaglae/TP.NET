using System;

namespace BookLibrary.Server.Dtos;

public class GetGenresResponse
{
    public ArraySegment<GenreDto> Data { get; set; }
    public bool HasMore { get; set; }
}
using System;

namespace BookLibrary.Server.Dtos;

public class GetAuthorsResponse
{
    public ArraySegment<AuthorDto> Data { get; set; }
    public bool HasMore { get; set; }
}
using System;
using System.Linq;
using BookLibrary.Server.Dtos;

namespace BookLibrary.Server.Models.Api;

public class GetAuthorsResponse
{
    public ArraySegment<AuthorDto> Data { get; set; }
    public bool HasMore { get; set; }
}
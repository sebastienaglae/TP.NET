using System;
using System.Linq;
using BookLibrary.Server.Dtos;

namespace BookLibrary.Server.Models.Api;

public class GetBooksResponse
{
    public ArraySegment<BookListEntryDto> Data { get; set; }
    public bool HasMore { get; set; }
}
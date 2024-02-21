using System;

namespace BookLibrary.Server.Dtos;

public class GetBooksResponse
{
    public ArraySegment<BookListEntryDto> Data { get; set; }
    public bool HasMore { get; set; }
}
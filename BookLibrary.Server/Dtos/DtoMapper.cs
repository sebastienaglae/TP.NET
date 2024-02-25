using System.Collections.Generic;
using System.Linq;
using BookLibrary.Server.Models;
using Riok.Mapperly.Abstractions;

namespace BookLibrary.Server.Dtos;

[Mapper]
public partial class DtoMapper
{
    [MapProperty(nameof(Book.Id), nameof(BookDto.Id))]
    [MapProperty(nameof(Book.Name), nameof(BookDto.Title))]
    [MapProperty(nameof(Book.Authors), nameof(BookDto.Authors))]
    [MapProperty(nameof(Book.Description), nameof(BookDto.Description))]
    [MapProperty(nameof(Book.Pages), nameof(BookDto.Pages))]
    [MapProperty(nameof(Book.Price), nameof(BookDto.Price))]
    [MapProperty(nameof(Book.Genres), nameof(BookDto.Genres))]
    public partial BookDto ToBookDto(Book book);

    public partial IQueryable<BookDto> ToBookDto(IQueryable<Book> books);

    [MapProperty(nameof(Genre.Id), nameof(BookGenreDto.Id))]
    [MapProperty(nameof(Genre.Name), nameof(BookGenreDto.Name))]
    public partial BookGenreDto ToBookGenreDto(Genre genre);

    public partial GenreDto ToGenreDto(Genre genres);
    public partial GenreDto[] ToGenreDto(IEnumerable<Genre> genres);

    [MapProperty(nameof(Book.Id), nameof(BookListEntryDto.Id))]
    [MapProperty(nameof(Book.Name), nameof(BookListEntryDto.Title))]
    [MapProperty(nameof(Book.Authors), nameof(BookListEntryDto.Authors))]
    [MapProperty(nameof(Book.Price), nameof(BookListEntryDto.Price))]
    public partial BookListEntryDto ToBookListEntryDto(Book book);

    public partial BookListEntryDto[] ToBookListEntryDto(IEnumerable<Book> books);

    public partial AuthorDto ToAuthorDto(Author author);

    public partial AuthorDto[] ToAuthorDto(IEnumerable<Author> authors);
}
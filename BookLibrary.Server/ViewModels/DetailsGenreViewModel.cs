using System.Collections;
using System.Collections.Generic;
using BookLibrary.Server.Models;

namespace BookLibrary.Server.ViewModels;

public class DetailsGenreViewModel
{
    public int Id { get; init; }
    public string Name { get; init; }
    public ICollection<Book> Books { get; init; }
}
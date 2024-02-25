using System.Collections;
using System.Collections.Generic;
using BookLibrary.Server.Models;

namespace BookLibrary.Server.ViewModels;

public class DetailsAuthorViewModel
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public ICollection<Book> Books { get; init; }
}
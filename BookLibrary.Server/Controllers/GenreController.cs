using Microsoft.AspNetCore.Mvc;
using BookLibrary.Server.Database;

namespace BookLibrary.Server.Controllers;

public class GenreController(LibraryDbContext libraryDbContext) : Controller
{
    private readonly LibraryDbContext libraryDbContext = libraryDbContext;

    // A vous de faire comme BookController.List mais pour les genres !
}
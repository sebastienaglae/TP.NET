using BookLibrary.Server.Database;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Server.Controllers;

public class GenreController : Controller
{
    private readonly LibraryDbContext _context;

    public GenreController(LibraryDbContext libraryDbContext)
    {
        _context = libraryDbContext;
    }

    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Edit(int id)
    {
        return View();
    }

    public IActionResult List()
    {
        return View();
    }

    public IActionResult Details(int id)
    {
        return View();
    }
}
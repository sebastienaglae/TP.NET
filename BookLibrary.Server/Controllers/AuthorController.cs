using BookLibrary.Server.Database;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Server.Controllers;

public class AuthorController : Controller
{
    private readonly LibraryDbContext _context;

    public AuthorController(LibraryDbContext context)
    {
        _context = context;
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
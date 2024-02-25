using BookLibrary.Server.Database;
using BookLibrary.Server.Models;
using BookLibrary.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibrary.Server.Controllers;

[Authenticate]
public class GenreController : Controller
{
    private readonly LibraryDbContext _context;

    public GenreController(LibraryDbContext libraryDbContext)
    {
        _context = libraryDbContext;
    }

    [Authenticate(AdminRole.AddGenres)]
    public IActionResult Create()
    {
        return View();
    }

    [Authenticate(AdminRole.EditGenres)]
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
    
    [Authenticate(AdminRole.DeleteGenres)]
    public IActionResult Delete(int id)
    {
        throw new System.NotImplementedException();
    }
}
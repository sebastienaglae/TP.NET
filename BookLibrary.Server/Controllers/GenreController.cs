using System;
using System.Linq;
using System.Threading.Tasks;
using BookLibrary.Server.Database;
using BookLibrary.Server.Models;
using BookLibrary.Server.Services;
using BookLibrary.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    
    [HttpPost]
    [Authenticate(AdminRole.AddGenres)]
    public async Task<IActionResult> Create(CreateGenreViewModel model)
    {
        if (ModelState.IsValid)
        {
            _context.Genres.Add(new Genre
            {
                Name = model.Name
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }

        return View(model);
    }

    [Authenticate(AdminRole.EditGenres)]
    public async Task<IActionResult> Edit(int id)
    {
        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
        if (genre == null)
            return NotFound();
        
        return View(new EditGenreViewModel
        {
            Id = genre.Id,
            Name = genre.Name
        });
    }
    
    [HttpPost]
    [Authenticate(AdminRole.EditGenres)]
    public async Task<IActionResult> Edit(EditGenreViewModel model)
    {
        if (ModelState.IsValid)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == model.Id);
            if (genre == null)
                return NotFound();
            
            genre.Name = model.Name;
            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }

        return View(model);
    }

    public async Task<IActionResult> List(int itemsPerPage = 10, int page = 1)
    {
        itemsPerPage = Math.Max(itemsPerPage, 1);
        itemsPerPage = Math.Min(itemsPerPage, 100);
        page = Math.Max(page, 1);

        var totalPages = await GetTotalPageAsync(itemsPerPage);

        page = Math.Min(page, totalPages);

        var genres = _context.Genres
            .OrderBy(b => b.Name)
            .Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .Include(b => b.Books);

        var booksResults = await genres.ToListAsync();
        var paginationResults = new PaginationInfo(page, itemsPerPage, totalPages);
        var model = new GenreResultsViewModel(booksResults, paginationResults);

        return View(model);
    }
    
    private async Task<int> GetTotalPageAsync(int itemsPerPage)
    {
        int genresCount = await _context.Genres.CountAsync();
        return genresCount / itemsPerPage + (genresCount % itemsPerPage == 0 ? 0 : 1);
    }

    public async Task<IActionResult> Details(int id)
    {
        var genre = await _context.Genres
            .Include(g => g.Books)
            .FirstOrDefaultAsync(g => g.Id == id);
        if (genre == null)
            return NotFound();
        
        return View(new DetailsGenreViewModel
        {
            Id = genre.Id,
            Name = genre.Name,
            Books = genre.Books
        });
    }
    
    [Authenticate(AdminRole.DeleteGenres)]
    public async Task<IActionResult> Delete(int id)
    {
        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
        if (genre == null)
            return NotFound();
        
        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
        return RedirectToAction("List");
    }
}
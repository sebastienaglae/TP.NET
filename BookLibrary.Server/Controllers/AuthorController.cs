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
public class AuthorController : Controller
{
    private readonly LibraryDbContext _context;

    public AuthorController(LibraryDbContext context)
    {
        _context = context;
    }

    [Authenticate(AdminRole.AddAuthors)]
    public IActionResult Create()
    {
        return View();
    }
    
    [HttpPost]
    [Authenticate(AdminRole.AddAuthors)]
    public async Task<IActionResult> Create(CreateAuthorViewModel model)
    {
        if (ModelState.IsValid)
        {
            _context.Authors.Add(new Author
            {
                FirstName = model.FirstName,
                LastName = model.LastName
            });
            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }

        return View(model);
    }

    [Authenticate(AdminRole.EditAuthors)]
    public async Task<IActionResult> Edit(int id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
        if (author == null)
            return NotFound();
        
        return View(new EditAuthorViewModel
        {
            Id = author.Id,
            FirstName = author.FirstName,
            LastName = author.LastName
        });
    }
    
    [HttpPost]
    [Authenticate(AdminRole.EditAuthors)]
    public async Task<IActionResult> Edit(EditAuthorViewModel author)
    {
        if (ModelState.IsValid)
        {
            var authorEntity = await _context.Authors.FirstOrDefaultAsync(a => a.Id == author.Id);
            if (authorEntity == null)
                return NotFound();
            
            authorEntity.FirstName = author.FirstName;
            authorEntity.LastName = author.LastName;
            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }

        return View(author);
    }

    public async Task<IActionResult> List(int itemsPerPage = 10, int page = 1)
    {
        itemsPerPage = Math.Max(itemsPerPage, 1);
        itemsPerPage = Math.Min(itemsPerPage, 100);
        page = Math.Max(page, 1);

        var totalPages = await GetTotalPageAsync(itemsPerPage);

        page = Math.Min(page, totalPages);

        var authors = _context.Authors
            .OrderBy(b => b.FirstName)
            .Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .Include(b => b.Books);

        var booksResults = await authors.ToListAsync();
        var paginationResults = new PaginationInfo(page, itemsPerPage, totalPages);
        var model = new AuthorResultsViewModel(booksResults, paginationResults);

        return View(model);
    }
    
    private async Task<int> GetTotalPageAsync(int itemsPerPage)
    {
        int authorsCount = await _context.Authors.CountAsync();
        return authorsCount / itemsPerPage + (authorsCount % itemsPerPage == 0 ? 0 : 1);
    }

    public async Task<IActionResult> Details(int id)
    {
        var author = _context.Authors
            .Include(a => a.Books)
            .FirstOrDefault(a => a.Id == id);
        if (author == null)
            return NotFound();
        
        return View(new DetailsAuthorViewModel
        {
            FirstName = author.FirstName,
            LastName = author.LastName,
            Books = author.Books
        });
    }
    
    [Authenticate(AdminRole.DeleteAuthors)]
    public async Task<IActionResult> Delete(int id)
    {
        var author = await _context.Authors.FirstOrDefaultAsync(a => a.Id == id);
        if (author == null)
            return NotFound();
        
        _context.Authors.Remove(author);
        await _context.SaveChangesAsync();
        return RedirectToAction("List");
    }
}
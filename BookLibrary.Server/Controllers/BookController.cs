using System;
using System.Linq;
using System.Threading.Tasks;
using BookLibrary.Server.Database;
using BookLibrary.Server.Dtos;
using BookLibrary.Server.Models;
using BookLibrary.Server.Services;
using BookLibrary.Server.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Server.Controllers;

[Authenticate]
public class BookController : Controller
{
    private readonly LibraryDbContext _context;
    private readonly OpenLibraryService _openLibraryService;

    public BookController(LibraryDbContext context, OpenLibraryService openLibraryService)
    {
        _context = context;
        _openLibraryService = openLibraryService;
    }

    public async Task<IActionResult> List(int itemsPerPage = 10, int page = 1)
    {
        itemsPerPage = Math.Max(itemsPerPage, 1);
        itemsPerPage = Math.Min(itemsPerPage, 100);
        page = Math.Max(page, 1);

        var totalPages = await GetTotalPageAsync(itemsPerPage, _context.Books);

        page = Math.Min(page, totalPages);

        var books = _context.Books
            .OrderBy(b => b.Name)
            .Skip((page - 1) * itemsPerPage)
            .Take(itemsPerPage)
            .Include(b => b.Authors)
            .Include(b => b.Genres);

        var booksResults = await books.ToListAsync();
        var paginationResults = new PaginationInfo(page, itemsPerPage, totalPages);
        var model = new BookResultsViewModel(booksResults, paginationResults);

        return View(model);
    }
    
    public async Task<IActionResult> Search([FromQuery] BookSearchResultsViewModel filter)
    {
        var totalPages = await GetTotalPageAsync(filter.ItemsPerPage, ApplyFilter(_context.Books, filter));

        filter.Page = Math.Min(filter.Page, totalPages);

        var books = ApplyFilter(_context.Books, filter)
            .OrderBy(b => b.Name)
            .Skip((filter.Page - 1) * filter.ItemsPerPage)
            .Take(filter.ItemsPerPage)
            .Include(b => b.Authors)
            .Include(b => b.Genres);

        var booksResults = await books.ToListAsync();
        var paginationResults = new PaginationInfo(filter.Page, filter.ItemsPerPage, totalPages);
        var model = new BookResultsViewModel(booksResults, paginationResults);

        return View("_ListTable", model);
        
        static IQueryable<Book> ApplyFilter(IQueryable<Book> books, BookSearchResultsViewModel filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Title))
                books = books.Where(b => b.Name.Contains(filter.Title));

            if (!string.IsNullOrWhiteSpace(filter.Author))
                books = books.Where(b => b.Authors.Any(a => a.FirstName.Contains(filter.Author) || a.LastName.Contains(filter.Author)));

            if (!string.IsNullOrWhiteSpace(filter.Genre))
                books = books.Where(b => b.Genres.Any(g => g.Name.Contains(filter.Genre)));

            return books;
        }
    }

    private async Task<int> GetTotalPageAsync(int itemsPerPage, IQueryable<Book> books)
    {
        int booksCount = await books.CountAsync();
        return booksCount / itemsPerPage + (booksCount % itemsPerPage == 0 ? 0 : 1);
    }

    [Authenticate(AdminRole.AddBooks)]
    public IActionResult Create()
    {
        var model = new CreateBookViewModel
        {
            Name = "Book Name",
            Description = "Book Content",
            Price = 10.0m
        };

        return View(model);
    }

    [HttpPost]
    [Authenticate(AdminRole.AddBooks)]
    public async Task<IActionResult> Create(CreateBookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            FillAuthorsAndGenres(model);
            return View(model);
        }

        var book = new Book
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            Genres = await _context.Genres.Where(g => model.GenreIds.Contains(g.Id)).ToListAsync(),
            Authors = await _context.Authors.Where(a => model.AuthorIds.Contains(a.Id)).ToListAsync()
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return RedirectToAction("List");
    }

    [Authenticate(AdminRole.DeleteBooks)]
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return NotFound();

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return RedirectToAction("List");
    }

    [Authenticate(AdminRole.EditBooks)]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _context.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (book is null) return NotFound();

        var model = new EditBookViewModel
        {
            Id = book.Id,
            Name = book.Name,
            Description = book.Description,
            Price = book.Price,
            GenreIds = book.Genres.Select(g => g.Id).ToList(),
            AuthorIds = book.Authors.Select(a => a.Id).ToList()
        };

        FillAuthorsAndGenres(model);

        return View(model);
    }

    private void FillAuthorsAndGenres(CreateBookViewModel model)
    {
        if (model.AuthorIds is not null)
            model.Authors = _context.Authors.Where(a => model.AuthorIds.Contains(a.Id)).ToList()
                .Select(a => new IdKeyValue { Key = a.Id, Value = a.FirstName + " " + a.LastName }).ToList();

        if (model.GenreIds is not null)
            model.Genres = _context.Genres.Where(g => model.GenreIds.Contains(g.Id)).ToList()
                .Select(g => new IdKeyValue { Key = g.Id, Value = g.Name }).ToList();
    }

    [HttpPost]
    [Authenticate(AdminRole.EditBooks)]
    public async Task<IActionResult> Edit(EditBookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            FillAuthorsAndGenres(model);
            return View(model);
        }

        var book = await _context.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .FirstOrDefaultAsync(b => b.Id == model.Id);
        if (book is null) return NotFound();

        book.Name = model.Name;
        book.Description = model.Description;
        book.Price = model.Price;
        book.Genres = _context.Genres.Where(g => model.GenreIds.Contains(g.Id)).ToList();
        book.Authors = _context.Authors.Where(a => model.AuthorIds.Contains(a.Id)).ToList();

        _context.Books.Update(book);
        await _context.SaveChangesAsync();

        return RedirectToAction("List");
    }
    
    [HttpPost]
    [Authenticate(AdminRole.EditBooks)]
    public async Task<ActionResult<ImportBookResponse>> Import(int bookId, string isbn)
    {
        var book = await _context.Books.FindAsync(bookId);
        if (book is null)
            return NotFound();

        var openLibraryBook = await _openLibraryService.GetBookAsync(isbn);
        if (openLibraryBook is null)
            return BadRequest("Invalid ISBN.");
        
        

        return null;
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _context.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .FirstOrDefaultAsync(b => b.Id == id);
        if (book is null) return NotFound();

        var model = new DetailsBookViewModel
        {
            Id = book.Id,
            Name = book.Name,
            Description = book.Description,
            Price = book.Price,
            Authors = book.Authors.Select(a => a.FirstName + " " + a.LastName).ToList(),
            Genres = book.Genres.Select(g => g.Name).ToList()
        };

        return View(model);
    }
}
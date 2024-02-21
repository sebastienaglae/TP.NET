using System;
using System.Linq;
using System.Threading.Tasks;
using BookLibrary.Server.Database;
using BookLibrary.Server.Models;
using BookLibrary.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLibrary.Server.Controllers;

public class BookController : Controller
{
    private readonly LibraryDbContext _context;

    public BookController(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> List(int itemsPerPage = 10, int page = 1)
    {
        itemsPerPage = Math.Max(itemsPerPage, 1);
        itemsPerPage = Math.Min(itemsPerPage, 100);
        page = Math.Max(page, 1);

        var totalPages = GetTotalPage(itemsPerPage);

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

    private int GetTotalPage(int itemsPerPage)
    {
        return (int)Math.Ceiling(_context.Books.Count() / (double)itemsPerPage);
    }

    public IActionResult Create()
    {
        var model = new CreateBookViewModel
        {
            Name = "Book Name",
            Content = "Book Content",
            Price = 10.0m
        };

        return View(model);
    }

    [HttpPost]
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
            Content = model.Content,
            Price = model.Price,
            Genres = _context.Genres.Where(g => model.GenreIds.Contains(g.Id)).ToList(),
            Authors = _context.Authors.Where(a => model.AuthorIds.Contains(a.Id)).ToList()
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return RedirectToAction("List");
    }

    public IActionResult Delete(int id)
    {
        var book = _context.Books.Find(id);
        if (book is null) return NotFound();

        _context.Books.Remove(book);
        _context.SaveChanges();

        return RedirectToAction("List");
    }

    public IActionResult Edit(int id)
    {
        var book = _context.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .FirstOrDefault(b => b.Id == id);
        if (book is null) return NotFound();

        var model = new EditBookViewModel
        {
            Id = book.Id,
            Name = book.Name,
            Content = book.Content,
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
    public async Task<IActionResult> Edit(EditBookViewModel model)
    {
        if (!ModelState.IsValid)
        {
            FillAuthorsAndGenres(model);
            return View(model);
        }

        var book = _context.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .FirstOrDefault(b => b.Id == model.Id);

        if (book is null) return NotFound();

        book.Name = model.Name;
        book.Content = model.Content;
        book.Price = model.Price;
        book.Genres = _context.Genres.Where(g => model.GenreIds.Contains(g.Id)).ToList();
        book.Authors = _context.Authors.Where(a => model.AuthorIds.Contains(a.Id)).ToList();

        _context.Books.Update(book);
        await _context.SaveChangesAsync();

        return RedirectToAction("List");
    }

    public IActionResult Details(int id)
    {
        var book = _context.Books
            .Include(b => b.Authors)
            .Include(b => b.Genres)
            .FirstOrDefault(b => b.Id == id);
        if (book is null) return NotFound();

        var model = new DetailsBookViewModel
        {
            Id = book.Id,
            Name = book.Name,
            Content = book.Content,
            Price = book.Price,
            Authors = book.Authors.Select(a => a.FirstName + " " + a.LastName).ToList(),
            Genres = book.Genres.Select(g => g.Name).ToList()
        };

        return View(model);
    }
}
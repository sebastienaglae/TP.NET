using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookLibrary.Server.Database;
using BookLibrary.Server.Services;
using BookLibrary.Server.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Server.Controllers;

[Authenticate]
public class StatisticsController(ILogger<StatisticsController> logger, LibraryDbContext dbContext, IMemoryCache cache) : Controller
{
    public async Task<IActionResult> Index()
    {
        if (!cache.TryGetValue("Statistics", out StatisticsViewModel viewModel))
        {
            var countTasks = new[]
            {
                dbContext.Books.CountAsync(),
                dbContext.Authors.CountAsync(),
                dbContext.Genres.CountAsync()
            };
            var (booksCount, authorsCount, genresCount) = (await countTasks[0], await countTasks[1], await countTasks[2]);

            Task<int>[] minMedMaxBookTasks;
            Task<decimal>[] minMedMaxBookPriceTasks;
            // EF doesn't support Sum(s => s.Length), so we have to do plain LINQ (which is not supported by in-memory database)
            if (dbContext.Database.IsInMemory())
            {
                minMedMaxBookTasks = new []
                {
                    dbContext.Books.ToListAsync().ContinueWith(t => t.Result.Min(b => b.Pages.Sum(p => p.Length))),
                    dbContext.Books.ToListAsync().ContinueWith(t => t.Result.OrderBy(b => b.Pages.Sum(p => p.Length)).Select(b => b.Pages.Sum(p => p.Length)).Skip(booksCount / 2).FirstOrDefault()),
                    dbContext.Books.ToListAsync().ContinueWith(t => t.Result.Max(b => b.Pages.Sum(p => p.Length)))
                };
                minMedMaxBookPriceTasks = new []
                {
                    dbContext.Books.ToListAsync().ContinueWith(t => t.Result.Min(b => b.Price)),
                    dbContext.Books.ToListAsync().ContinueWith(t => t.Result.OrderBy(b => b.Price).Select(b => b.Price).Skip(booksCount / 2).FirstOrDefault()),
                    dbContext.Books.ToListAsync().ContinueWith(t => t.Result.Max(b => b.Price))
                };
            }
            else
            {
                throw new NotImplementedException("This is not implemented");
            }
            var authorsBooksCountTask = dbContext.Authors
                .Include(a => a.Books)
                .ToDictionaryAsync(a => a, a => a.Books.Count);
            var genresBooksCountTask = dbContext.Genres
                .Include(g => g.Books)
                .ToDictionaryAsync(g => g, g => g.Books.Count);
            
            viewModel = new StatisticsViewModel(booksCount, authorsCount, genresCount, await authorsBooksCountTask, await genresBooksCountTask,
                await minMedMaxBookTasks[0], await minMedMaxBookTasks[1], await minMedMaxBookTasks[2],
                await minMedMaxBookPriceTasks[0], await minMedMaxBookPriceTasks[1], await minMedMaxBookPriceTasks[2]);
            
            cache.Set("Statistics", viewModel, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            });
        }

        return View(viewModel);
    }
}
using System;
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
            
            var minMedMaxBookTasks = new []
            {
                dbContext.Books.MinAsync(b => b.Pages.Sum(p => p.Length)),
                dbContext.Books.OrderBy(b => b.Pages.Sum(p => p.Length)).Select(b => b.Pages.Sum(p => p.Length)).Skip(booksCount / 2).FirstOrDefaultAsync(),
                dbContext.Books.MaxAsync(b => b.Pages.Sum(p => p.Length))
            };
            var minMedMaxBookPriceTasks = new []
            {
                dbContext.Books.MinAsync(b => b.Price),
                dbContext.Books.OrderBy(b => b.Price).Select(b => b.Price).Skip(booksCount / 2).FirstOrDefaultAsync(),
                dbContext.Books.MaxAsync(b => b.Price)
            };
            var authorsBooksCountTask = dbContext.Authors
                .Include(a => a.Books)
                .ToDictionaryAsync(a => a, a => a.Books.Count);
            
            viewModel = new StatisticsViewModel(booksCount, authorsCount, genresCount, await authorsBooksCountTask,
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
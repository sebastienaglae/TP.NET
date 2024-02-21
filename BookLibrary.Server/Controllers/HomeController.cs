using System.Diagnostics;
using BookLibrary.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Server.Controllers;

public class HomeController(ILogger<HomeController> logger) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult StatusCode(string code)
    {
        return code switch
        {
            "404" => View("~/Views/404.cshtml"),
            "500" => View("~/Views/500.cshtml"),
            _ => View("Error")
        };
    }
}
using System.Diagnostics;
using BookLibrary.Server.Services;
using BookLibrary.Server.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookLibrary.Server.Controllers;

[Authenticate]
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
            "400" => View("~/Views/400.cshtml"),
            "404" => View("~/Views/404.cshtml"),
            "500" => View("~/Views/500.cshtml"),
            _ => View("Error")
        };
    }
    
    public IActionResult AccessDenied()
    {
        return View("~/Views/400.cshtml");
    }
}
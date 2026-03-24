using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RMS.Web.Models;

namespace RMS.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
     public IActionResult About()
    { 
        var about = new AboutViewModel {
            Title = "About",
            Message = "Our mission is to develop a Menu management system allowing users to look up/create and rate their favourite Menus.",
            Formed = new DateTime(2024,05,03)
        };
        return View(about);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

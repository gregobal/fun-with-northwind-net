using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;
using System.Diagnostics;
using Northwind.Shared;

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly NorthwindContext db;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, NorthwindContext northwindContext)
    {
        _logger = logger;
        db = northwindContext;
    }

    public IActionResult Index()
    {
        var model = new HomeIndexViewModel
            (
            VisitorCount: new Random().Next(1, 1001),
            Categories: db.Categories.ToList(),
            Products: db.Products.ToList()
            );

        return View(model);
    }

    public IActionResult ProductDetail(int? id) { 
        if (!id.HasValue)
        {
            return BadRequest("You must pass a product Id in the route");
        }

        var model = db.Products
            .SingleOrDefault(p => p.ProductId == id);

        if (model is null)
        {
            return NotFound($"Prodict with Id = {id} not found");
        }

        return View(model);
    }

    [Authorize(Roles = "Administrators")]
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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;
using Northwind.Shared;
using System.Diagnostics;

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

    public async Task<IActionResult> Index()
    {
        var model = new HomeIndexViewModel
            (
            VisitorCount: new Random().Next(1, 1001),
            Categories: await db.Categories.ToListAsync(),
            Products: await db.Products.ToListAsync()
            );

        return View(model);
    }

    public async Task<IActionResult> ProductDetail(int? id)
    {
        if (!id.HasValue)
        {
            return BadRequest("You must pass a product Id in the route");
        }

        var model = await db.Products
            .SingleOrDefaultAsync(p => p.ProductId == id);

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
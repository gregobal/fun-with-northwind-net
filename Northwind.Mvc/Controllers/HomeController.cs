using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;
using Northwind.Shared;
using System.Diagnostics;
using Grpc.Net.Client;

namespace Northwind.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly NorthwindContext db;
    private readonly IHttpClientFactory clientFactory;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, NorthwindContext northwindContext, 
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        db = northwindContext;
        clientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            using (var channel = GrpcChannel.ForAddress("https://localhost:5006"))
            {
                Greeter.GreeterClient greeter = new(channel);
                var reply = await greeter.SayHelloAsync(new HelloRequest { Name = "Baobab" });
                ViewData["greeting"] = reply.Message;
            }
            
        } catch (Exception)
        {
            _logger.LogWarning("Northwind.gRPC service is not responding.");
        }

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

    public async Task<IActionResult> Customers(string country)
    {
        string uri;

        if (string.IsNullOrEmpty(country))
        {
            ViewData["Title"] = "All Customers Worldwide";
            uri = "api/customers/";
        } else
        {
            ViewData["Title"] = $"Customers from {country}";
            uri = $"api/customers/?country={country}";
        }

        var client = clientFactory.CreateClient(name: "Northwind.WebApi");
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        var response = await client.SendAsync(request);

        var model = await response.Content.ReadFromJsonAsync<IEnumerable<Customer>>();

        return View(model);
    }

    public async Task<IActionResult> Shipper(int id)
    {
        try
        {
            using (var channel = GrpcChannel.ForAddress("https://localhost:5006"))
            {
                Shipr.ShiprClient shipr = new(channel);
                var result = await shipr.GetShipperAsync(new ShipperRequest { ShipperId = id });
                var model = new Shipper
                {
                    ShipperId = result.ShipperId,
                    CompanyName = result.CompanyName,
                    Phone = result.Phone
                };
                return View(model);
            }

        } catch (Exception) {
            _logger.LogWarning("Northwind.gRPC service is not responding.");
        }

        return NotFound();
    }

    public IActionResult Chat()
    {
        return View();
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
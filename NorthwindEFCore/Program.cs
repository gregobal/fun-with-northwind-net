using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using NorthwindEFCore;
using NorthwindEFCore.Models;


Console.WriteLine($"Using Microsoft sq server db provider");
QueryingCategories();
Console.WriteLine();
FilteredIncludes();

static void QueryingCategories()
{
    using(NorthwindDb db = new())
    {
        ILoggerFactory loggerFactory = db.GetService<ILoggerFactory>();
        loggerFactory.AddProvider(new ConsoleLoggerProvider());

        Console.WriteLine("Categories and how many they have:");

        IQueryable<Category>? categories = db.Categories?
            .TagWith("Get Categories with products. ")
            .Include(c => c.Products);


        if (categories is null)
        {
            Console.WriteLine("No categories found");
            return;
        }

        foreach (var c in categories)
        {
            Console.WriteLine($"{c.CategoryName} has {c.Products.Count} products");
        }
    }
}

static void FilteredIncludes()
{
    using (NorthwindDb db = new())
    {
        Console.WriteLine("Enter a minimum for units in stock: ");
        string unitsInStock = Console.ReadLine() ?? "10";
        int stock = int.Parse(unitsInStock);

        IQueryable<Category>? categories = db.Categories?
            .Include (c => c.Products.Where(p => p.Stock >= stock));

        if (categories is null)
        {
            Console.WriteLine("No categories found");
            return;
        }

        Console.WriteLine($"ToQueryString: {categories.ToQueryString()}");

        foreach (var c in categories)
        {
            Console.WriteLine($"{c.CategoryName} has {c.Products.Count} products with a minimum of {stock} units in stock.");
 
            foreach(var p in c.Products)
            {
                Console.WriteLine($"{p.ProductName} has {p.Stock} units in stock");
            }
        }
    }
}
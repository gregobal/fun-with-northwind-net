using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Northwind.Shared;

namespace NorthwindFeatures.Pages;

public class EmpoyeesPageModel : PageModel
{
    private NorthwindContext db;

    public EmpoyeesPageModel(NorthwindContext db)
    {
        this.db = db;
    }

    public Employee[] Employees { get; set; } = null!;

    public void OnGet()
    {
        ViewData["Title"] = "Northwind B2B - Employees";
        Employees = db.Employees
            .OrderBy(e => e.LastName)
            .ThenBy(e => e.FirstName)
            .ToArray();
    }
}
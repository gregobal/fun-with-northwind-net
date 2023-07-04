using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Northwind.Web.Pages
{
    public class SuppliersModel : PageModel
    {
        public IEnumerable<string>? Suppliers { get; set; }

        public void OnGet()
        {
            ViewData["Title"] = "Northwind B2B - Suppliers";

            Suppliers = new[]
            {
                "Alfa Digital", "Beta Limited", "Gamma Group"
            };
        }
    }
}

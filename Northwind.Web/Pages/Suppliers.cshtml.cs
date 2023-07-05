using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Northwind.Shared;

namespace Northwind.Web.Pages
{
    public class SuppliersModel : PageModel
    {
        private NorthwindContext db;

        public SuppliersModel(NorthwindContext db)
        {
            this.db = db;
        }

        public IEnumerable<Supplier>? Suppliers { get; set; }
        [BindProperty]
        public Supplier Supplier { get; set; }

        public void OnGet()
        {
            ViewData["Title"] = "Northwind B2B - Suppliers";

            Suppliers = db.Suppliers
                .OrderBy(s => s.Country)
                .ThenBy(s => s.CompanyName);
        }

        public IActionResult OnPost()
        {
            if ((Supplier is not null) && ModelState.IsValid)
            {
                db.Suppliers.Add(Supplier);
                db.SaveChanges();
                return RedirectToPage("/suppliers");
            } else
            {
                return Page();
            }
        }
    }
}

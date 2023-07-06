using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Northwind.Mvc.Controllers;

public class RolesController : Controller
{
    private string AdminRole = "Administrators";
    private string UserEmail = "test@example.com";

    private readonly RoleManager<IdentityRole> roleManager;
    private readonly UserManager<IdentityUser> userManager;

    public RolesController(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        this.roleManager = roleManager;
        this.userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        if (!(await roleManager.RoleExistsAsync(AdminRole)))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRole));
        }

        var user = await userManager.FindByEmailAsync(UserEmail);

        if (user is null)
        {
            user = new();
            user.UserName = UserEmail;
            user.Email = UserEmail;

            var result = await userManager.CreateAsync(user, "P@ssw0rd");

            if (result.Succeeded)
            {
                Console.WriteLine($"User {user.UserName} created successfully");
            } else
            {
                foreach(var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Description}");
                }
            }
        }

        if (!user.EmailConfirmed)
        {
            var token = await userManager
                .GenerateEmailConfirmationTokenAsync(user);
            var result = await userManager
                .ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                Console.WriteLine($"User {user.UserName} email confirmed successfully");
            } else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Description}");
                }
            }
        }

        if (!(await userManager.IsInRoleAsync(user, AdminRole))) 
        {
            var result = await userManager
                .AddToRoleAsync(user, AdminRole);

            if (result.Succeeded)
            {
                Console.WriteLine($"User {user.UserName} added to {AdminRole} successfully");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Description}");
                }
            }
        }

        return Redirect("/");
    }
}

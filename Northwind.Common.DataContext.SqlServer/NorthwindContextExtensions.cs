using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Shared;

public static class NorthwindContextExtensions
{
    public static IServiceCollection AddNorthwindContext(this IServiceCollection services,
        string connectionString = "Data Source=DESKTOP-B9CIR5M\\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=true;TrustServerCertificate=True")
    {
        services.AddDbContext<NorthwindContext>(options => options.UseSqlServer(connectionString));

        return services;
    }
}

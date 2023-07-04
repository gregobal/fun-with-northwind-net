using Microsoft.EntityFrameworkCore;
using NorthwindEFCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindEFCore;

public class NorthwindDb: DbContext
{
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Product>? Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connection = "Data Source=DESKTOP-B9CIR5M\\SQLEXPRESS;Initial Catalog=Northwind;Integrated Security=true;TrustServerCertificate=True;MultipleActiveResultSets=true";

        optionsBuilder.UseSqlServer(connection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .Property(category => category.CategoryName)
            .IsRequired()
            .HasMaxLength(15);

        modelBuilder.Entity<Product>()
            .HasQueryFilter(p => !p.Discontinued);
    }
}

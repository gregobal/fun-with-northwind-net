using Microsoft.EntityFrameworkCore;
using Northwind.Shared;

namespace Northwind.BlazorServer.Data;

public class NorthwindService : INorthwindService
{
    private readonly NorthwindContext db;

    public NorthwindService(NorthwindContext db)
    {
        this.db = db;
    }

    public Task<List<Customer>> GetCustomersAsync()
    {
        return db.Customers.ToListAsync();
    }

    public Task<List<Customer>> GetCustomersAsync(string country)
    {
        return db.Customers.Where(c => c.Country == country).ToListAsync();
    }

    public Task<Customer?> GetCustomerAsync(string id)
    {
        return db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
    }

    public Task<Customer> CreateCustomerAsync(Customer c)
    {
        db.Customers.Add(c);
        db.SaveChangesAsync();
        return Task.FromResult(c);
    }

    public Task<Customer> UpdateCustomerAsync(Customer c)
    {
        db.Entry(c).State = EntityState.Modified;
        db.SaveChangesAsync();
        return Task.FromResult(c);
    }

    public Task DeleteCustomerAsync(string id)
    {
        var customer = db.Customers.FirstOrDefaultAsync(c => c.CustomerId == id).Result;

        if (customer is null)
        {
            return Task.CompletedTask;
        }

        db.Customers.Remove(customer);
        return db.SaveChangesAsync();
    }
}

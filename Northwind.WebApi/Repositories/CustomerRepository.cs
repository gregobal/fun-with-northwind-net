using Northwind.Shared;
using System.Collections.Concurrent;

namespace Northwind.WebApi.Repositories;

public class CustomerRepository: ICustomerRepository
{
    private static ConcurrentDictionary<string, Customer> customersCache = new();
        
    private NorthwindContext db;

    public CustomerRepository(NorthwindContext db)
    {
        this.db = db;

        if (customersCache.IsEmpty)
        {
            customersCache = new ConcurrentDictionary<string, Customer>(
                db.Customers.ToDictionary(c => c.CustomerId));
        }
    }

    public async Task<Customer?> CreateAsync(Customer c)
    {
        c.CustomerId = c.CustomerId.ToUpper();

        var added = await db.Customers.AddAsync(c);
        var affected = await db.SaveChangesAsync();

        if (affected == 1) {            
            return customersCache.AddOrUpdate(c.CustomerId, c, UpdateCache);
        }

        return null;
    }

    public Task<IEnumerable<Customer>> GetAllAsync()
    {
        return Task.FromResult(customersCache is null ? Enumerable.Empty<Customer>() : 
            customersCache.Values);
    }

    public Task<Customer?> GetAsync(string id)
    {
        id = id.ToUpper();

        customersCache.TryGetValue(id, out var result);
        return Task.FromResult(result);
    }

    public async Task<Customer?> UpdateAsync(string id, Customer c)
    {
        id = id.ToUpper();
        c.CustomerId = c.CustomerId.ToUpper();

        db.Customers.Update(c);
        var affected = await db.SaveChangesAsync();

        if (affected == 1)
        {
            return UpdateCache(id, c);
        }
        
        return null;
    }

    public async Task<bool?> DeleteAsync(string id)
    {
        id = id.ToUpper();

        var c = db.Customers.Find(id);

        if (c is null) return null;

        db.Customers.Remove(c);
        var affected = await db.SaveChangesAsync();
        if (affected == 1)
        {
            return customersCache.TryRemove(id, out _);
        }

        return null;
    }

    private Customer UpdateCache(string id, Customer c)
    {
        Customer? old;
        if (customersCache.TryGetValue(id, out old))
        {
            if (customersCache.TryUpdate(id, c, old)) return c;
        }

        return null!;
    }
}

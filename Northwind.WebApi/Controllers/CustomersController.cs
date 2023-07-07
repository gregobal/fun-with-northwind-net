using Microsoft.AspNetCore.Mvc;
using Northwind.Shared;
using Northwind.WebApi.Repositories;

namespace Northwind.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerRepository repo;

    public CustomersController(ICustomerRepository repo)
    {
        this.repo = repo;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Customer>))]
    public async Task<IEnumerable<Customer>> GetCustomers(string? country)
    {
        var customers = await repo.GetAllAsync();

        if (string.IsNullOrWhiteSpace(country))
        {
            return customers;
        }

        return customers.Where(x => x.Country == country);
    }

    [HttpGet("{id}", Name = nameof(GetCustomer))]
    [ProducesResponseType(200, Type = typeof(Customer))]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetCustomer(string id)
    {
        var c = await repo.GetAsync(id);

        if (c is null) { 
            return NotFound(); 
        }

        return Ok(c);
    }

    [HttpPost]
    [ProducesResponseType(201, Type = typeof(Customer))]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] Customer c)
    {
        if (c is null)
        {
            return BadRequest();
        }

        var added = await repo.CreateAsync(c);

        if (added is null)
        {
            return BadRequest("Repository failed to create customer.");
        }

        return CreatedAtRoute(
            routeName: nameof(GetCustomer),
            routeValues: new { id = added.CustomerId },
            value: added);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(string id, [FromBody] Customer c)
    {
        id = id.ToUpper();
        c.CustomerId = c.CustomerId.ToUpper();

        if (c is null || c.CustomerId != id) {
            return BadRequest();
        }

        var existing = await repo.GetAsync(id);
        if (existing is null)
        {
            return NotFound();
        }

        await repo.UpdateAsync(id, c);

        return new NoContentResult();
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string id)
    {
        var existing = await repo.GetAsync(id);
        if (existing is null)
        {
            return NotFound(id);
        }

        var deleted = await repo.DeleteAsync(id);

        if (deleted.HasValue && deleted.Value)
        {
            return new NoContentResult();
        }

        return BadRequest($"Customer with Id = {id} was found but failed to delete");
    }
}
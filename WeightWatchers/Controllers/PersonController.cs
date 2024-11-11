using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeightWatchers.Data.Models;

namespace WeightWatchers.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController(WeightWatchersContext context) : ControllerBase
{
    private readonly WeightWatchersContext _context = context;

    [HttpGet]
    [Route("get-persons")]
    public async Task<List<Person>> GetPersons()
    {
        var persons = await _context.Person.ToListAsync();
        return persons;
    }

    [HttpPost]
    [Route("create-person")]
    public async Task CreatePerson([FromBody] Person person)
    {
        Person createPerson= new()
        {
            FirstName = person.FirstName,
            LastName = person.LastName
        };
        await _context.Person.AddAsync(createPerson);
        await _context.SaveChangesAsync();
    }

    [HttpPut]
    [Route("update-person")]
    public async Task UpdatePerson([FromBody] Person person)
    {
        var updatePerson = await _context.Person.Where(x => x.PersonId == person.PersonId).FirstOrDefaultAsync();
        if (updatePerson != null)
        {
            updatePerson.FirstName = person.FirstName;
            updatePerson.LastName = person.LastName;
            _context.Person.Update(updatePerson);
            await _context.SaveChangesAsync();
        }
    }

    [HttpDelete]
    [Route("delete-person/{personId}")]
    public async Task DeletePerson(int personId)
    {
        var deleteWeights = await _context.Weight.Where(x => x.PersonId == personId).ToListAsync();
        if(deleteWeights != null)
        {
            _context.Weight.RemoveRange(deleteWeights);
            await _context.SaveChangesAsync();
        }

        var deletePerson = await _context.Person.Where(x => x.PersonId == personId).FirstOrDefaultAsync();
        if(deletePerson != null)
        {
            _context.Person.Remove(deletePerson);
            await _context.SaveChangesAsync();
        }
    }
}
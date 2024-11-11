using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeightWatchers.Data.Models;

namespace WeightWatchers.Controllers;

[ApiController]
[Route("[controller]")]
public class WeightController(WeightWatchersContext context) : ControllerBase
{
    private readonly WeightWatchersContext _context = context;

    [HttpGet]
    [Route("get-weights/{personId}")]
    public async Task<List<Weight>> GetWeights(int personId)
    {
        var data = await _context.Weight.Where(x => x.PersonId == personId).ToListAsync();
        return data;
    }

    [HttpPost]
    [Route("create-weight")]
    public async Task CreateWeight([FromBody] Weight weight)
    {
        Weight createWeight = new()
        {
            Date = weight.Date,
            PersonId = weight.PersonId,
            Lbs = weight.Lbs,
            Kgs = weight.Kgs
        };
        await _context.Weight.AddAsync(createWeight);
        await _context.SaveChangesAsync();
    }

    [HttpPut]
    [Route("update-weight")]
    public async Task UpdateWeight([FromBody] Weight weight)
    {
        var updateWeight= await _context.Weight.Where(x => x.PersonId == weight.PersonId && x.Date == weight.Date).FirstOrDefaultAsync();
        if(updateWeight != null)
        {
            updateWeight.Lbs = weight.Lbs;
            updateWeight.Kgs = weight.Kgs;
            _context.Weight.Update(updateWeight);
            await _context.SaveChangesAsync();
        }
    }

    [HttpDelete]
    [Route("delete-weight/{personId}/{dateOnly}")]
    public async Task DeleteWeight(int personId, DateOnly dateOnly)
    {
        var deleteWeight= await _context.Weight.Where(x => x.PersonId == personId && x.Date == dateOnly).FirstOrDefaultAsync();
        if(deleteWeight != null)
        {
            _context.Weight.Remove(deleteWeight);
            await _context.SaveChangesAsync();
        }
    }
}
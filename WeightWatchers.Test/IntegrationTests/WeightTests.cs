using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using WeightWatchers.Data.Models;
using Xunit;

namespace WeightWatchers.Test.IntegrationTests;

public class WeightTests : IClassFixture<DockerWebApplicationFactoryFixture>
{
    private readonly DockerWebApplicationFactoryFixture _factory;
    private readonly HttpClient _httpClient;

    public static class Urls
    {
        public readonly static string GetWeights = "Weight/get-weights";
        public readonly static string CreateWeight = "Weight/create-weight";
        public readonly static string UpdateWeight = "Weight/update-weight";
        public readonly static string DeleteWeight =  "Weight/delete-weight";
    }

    public WeightTests(DockerWebApplicationFactoryFixture factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task GetWeights()
    {
        var response = await _httpClient.GetAsync($"{Urls.GetWeights}/1");
        var result = await response.Content.ReadFromJsonAsync<List<Weight>>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeOfType<List<Weight>>();
    }

    [Fact]
    public async Task CreateWeight()
    {
        var person = new Person() { FirstName = "Create", LastName = "Weight" };
        int personId = 0;

        using(var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var ctx = scopedServices.GetRequiredService<WeightWatchersContext>();
            await ctx.Person.AddAsync(person);
            await ctx.SaveChangesAsync();
            var dbPerson = await ctx.Person.Where(x => x.FirstName == person.FirstName && x.LastName == person.LastName).FirstAsync();
            personId = dbPerson.PersonId;
        }

        var weight = new Weight() { Date = DateOnly.FromDateTime(DateTime.Now), PersonId = personId, Lbs = 220.0M, Kgs = 100.0M };
        var response = await _httpClient.PostAsync(Urls.CreateWeight, HttpUtils.GetJsonHttpContent(weight));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using(var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var ctx = scopedServices.GetRequiredService<WeightWatchersContext>();
            var dbWeight= await ctx.Weight.Where(x => x.Date == weight.Date && x.PersonId == weight.PersonId).FirstAsync();
            dbWeight.Should().BeEquivalentTo(weight);
        }
    }

    [Fact]
    public async Task UpdateWeight()
    {
        var person = new Person() { FirstName = "Update", LastName = "Weight" };
        int personId = 0;
        var weight = new Weight() { Date = DateOnly.FromDateTime(DateTime.Now), PersonId = 0, Lbs = 0.0M, Kgs = 0.0M };

        using(var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var ctx = scopedServices.GetRequiredService<WeightWatchersContext>();
            await ctx.Person.AddAsync(person);
            await ctx.SaveChangesAsync();
            var dbPerson = await ctx.Person.Where(x => x.FirstName == person.FirstName && x.LastName == person.LastName).FirstAsync();
            personId = dbPerson.PersonId;
            weight.PersonId = personId;
            await ctx.Weight.AddAsync(weight);
            await ctx.SaveChangesAsync();
        }

        var updatedWeight = new Weight() { Date = weight.Date, PersonId = weight.PersonId, Lbs = 220.0M, Kgs = 100.0M };
        var response = await _httpClient.PutAsync(Urls.UpdateWeight, HttpUtils.GetJsonHttpContent(updatedWeight));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using(var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var ctx = scopedServices.GetRequiredService<WeightWatchersContext>();
            var dbWeight = await ctx.Weight.Where(x => x.Date == weight.Date && x.PersonId == weight.PersonId).FirstAsync();
            dbWeight.Should().BeEquivalentTo(updatedWeight);
        }
    }

    [Fact]
    public async Task DeleteWeight()
    {
        var person = new Person() { FirstName = "Delete", LastName = "Weight" };
        int personId = 0;
        var weight = new Weight() { Date = DateOnly.FromDateTime(DateTime.Now), PersonId = 0, Lbs = 0.0M, Kgs = 0.0M };

        using(var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var ctx = scopedServices.GetRequiredService<WeightWatchersContext>();
            await ctx.Person.AddAsync(person);
            await ctx.SaveChangesAsync();
            var dbPerson = await ctx.Person.Where(x => x.FirstName == person.FirstName && x.LastName == person.LastName).FirstAsync();
            personId = dbPerson.PersonId;
            weight.PersonId = personId;
            await ctx.Weight.AddAsync(weight);
            await ctx.SaveChangesAsync();
        }

        var response = await _httpClient.DeleteAsync($"{Urls.DeleteWeight}/{weight.PersonId}/{weight.Date}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using(var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var ctx = scopedServices.GetRequiredService<WeightWatchersContext>();
            var dbWeight = await ctx.Weight.Where(x => x.Date == weight.Date && x.PersonId == weight.PersonId).FirstOrDefaultAsync();
            dbWeight.Should().Be(null);
        }
    }
}
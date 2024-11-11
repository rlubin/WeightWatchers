using FluentAssertions;
using FluentAssertions.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using WeightWatchers.Data.Models;
using Xunit;

namespace WeightWatchers.Test.IntegrationTests;

public class PersonTests : IClassFixture<DockerWebApplicationFactoryFixture>
{
    private readonly DockerWebApplicationFactoryFixture _factory;
    private readonly HttpClient _httpClient;

    public static class Urls
    {
        public readonly static string GetPersons = "Person/get-persons";
        public readonly static string CreatePerson = "Person/create-person";
        public readonly static string UpdatePerson = "Person/update-person";
        public readonly static string DeletePerson =  "Person/delete-person";
    }

    public PersonTests(DockerWebApplicationFactoryFixture factory)
    {
        _factory = factory;
        _httpClient = _factory.CreateClient();
    }

    [Fact]
    public async Task GetPersons()
    {
        var response = await _httpClient.GetAsync(Urls.GetPersons);
        var result = await response.Content.ReadFromJsonAsync<List<Person>>();
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().BeOfType<List<Person>>();
    }

    [Fact]
    public async Task CreatePerson()
    {
        var person = new Person() { FirstName = "Create", LastName = "Person" };
        var response = await _httpClient.PostAsync(Urls.CreatePerson, HttpUtils.GetJsonHttpContent(person));
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using(var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var ctx = scopedServices.GetRequiredService<WeightWatchersContext>();
            var dbPerson = await ctx.Person.Where(x => x.FirstName == person.FirstName && x.LastName == person.LastName).FirstAsync();
            dbPerson.FirstName.Should().Be(person.FirstName);
            dbPerson.LastName.Should().Be(person.LastName);
        }
    }

    [Fact]
    public async Task UpdatePerson()
    {
        var person = new Person() { FirstName = "Update", LastName = "Person" };
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

        var updatedPerson = new Person() { PersonId = personId, FirstName = "Updated", LastName = "Person" };
        var response1 = await _httpClient.PutAsync(Urls.UpdatePerson, HttpUtils.GetJsonHttpContent(updatedPerson));
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        using(var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var ctx = scopedServices.GetRequiredService<WeightWatchersContext>();
            var dbPerson = await ctx.Person.Where(x => x.PersonId == personId).FirstAsync();
            dbPerson.Should().BeEquivalentTo(updatedPerson);
        }
    }

    [Fact]
    public async Task DeletePerson()
    {
        var person = new Person() { FirstName = "Delete", LastName = "Person" };
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

        var response1 = await _httpClient.DeleteAsync($"{Urls.DeletePerson}/{personId}");
        response1.StatusCode.Should().Be(HttpStatusCode.OK);

        using(var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var ctx = scopedServices.GetRequiredService<WeightWatchersContext>();
            var dbPerson = await ctx.Person.Where(x => x.PersonId == personId).FirstOrDefaultAsync();
            dbPerson.Should().Be(null);
        }
    }
}
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;
using WeightWatchers.Data.Models;
using Xunit;

namespace WeightWatchers.Test;
public class DockerWebApplicationFactoryFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private MsSqlContainer _dbContainer;

    public DockerWebApplicationFactoryFixture()
    {
        _dbContainer = new MsSqlBuilder().Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var connectionString = _dbContainer.GetConnectionString();
        base.ConfigureWebHost(builder);
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<WeightWatchersContext>));
            services.AddDbContext<WeightWatchersContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        using(var scope = Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var ctx = scopedServices.GetRequiredService<WeightWatchersContext>();

            await ctx.Database.EnsureCreatedAsync();
        }
    }

    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
}

using TaskMaster.Modules.Accounts.DAL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TaskMaster.Bootstrapper;
using Testcontainers.PostgreSql;
using Testcontainers.Redis;

namespace TaskMaster.Modules.Accounts.Tests.Integration.Helpers;

public class TestApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _pgContainer = new PostgreSqlBuilder()
        .WithDatabase("TaskMasterUsers")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .WithPortBinding(5432, 5432)
        .Build();
    private readonly RedisContainer _redisContainer = new RedisBuilder()
        .WithPortBinding(6379,6379)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var dbContextType = typeof(DbContextOptions<UsersDbContext>);
            var dbDescriptor = services.SingleOrDefault(s => s.ServiceType == dbContextType);
            if (dbDescriptor is not null)
                services.Remove(dbDescriptor);

            services.AddDbContext<UsersDbContext>(options => options.UseNpgsql(_pgContainer.GetConnectionString()));

            var redisType = typeof(IConnectionMultiplexer);
            var redisDescriptor = services.SingleOrDefault(s => s.ServiceType == redisType);
            if (redisDescriptor is not null)
                services.Remove(redisDescriptor);

            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(_redisContainer.GetConnectionString()));
        });
    }

    public async Task InitializeAsync()
    {
        await _pgContainer.StartAsync();
        await _redisContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _pgContainer.StopAsync();
        await _redisContainer.StopAsync();
    }
}
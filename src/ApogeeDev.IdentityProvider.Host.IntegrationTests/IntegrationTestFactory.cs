using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;
using Testcontainers.MongoDb;

namespace ApogeeDev.IdentityProvider.Host.IntegrationTests;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer;

    public IntegrationTestFactory()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IMongoClient));
            services.AddSingleton<IMongoClient>(sp =>
            {
                return new MongoClient(_mongoDbContainer.GetConnectionString());
            });
        });
        
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "App:MongoDbConnection", _mongoDbContainer.GetConnectionString() },
                { "App:DatabaseName", "TestDb" }
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mongoDbContainer.DisposeAsync();
    }
}

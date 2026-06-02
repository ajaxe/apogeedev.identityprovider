using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace ApogeeDev.IdentityProvider.Host.IntegrationTests;

public class ClientCredentialsFlowTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;

    public ClientCredentialsFlowTests(IntegrationTestFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task TokenEndpoint_WithValidClientCredentials_ReturnsAccessToken()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        
        var clientId = "test-client";
        var clientSecret = "test-secret";
        
        // Ensure client is created
        var existingClient = await manager.FindByClientIdAsync(clientId);
        if (existingClient == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
                }
            });
        }
        
        var client = _factory.CreateClient();
        
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret)
        });

        // Act
        var response = await client.PostAsync("/connect/token", requestContent);
        
        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("access_token", responseString);
    }

    [Fact]
    public async Task TokenEndpoint_WithInvalidClientSecret_ReturnsError()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        
        var clientId = "test-client-invalid-secret";
        var clientSecret = "test-secret";
        
        // Ensure client is created
        var existingClient = await manager.FindByClientIdAsync(clientId);
        if (existingClient == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
                }
            });
        }
        
        var client = _factory.CreateClient();
        
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", "wrong-secret")
        });

        // Act
        var response = await client.PostAsync("/connect/token", requestContent);
        
        // Assert
        Assert.False(response.IsSuccessStatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("error", responseString);
    }

    [Fact]
    public async Task TokenEndpoint_WithoutClientCredentialsGrant_ReturnsError()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        
        var clientId = "test-client-no-cc";
        var clientSecret = "test-secret";
        
        // Ensure client is created without Client Credentials permission
        var existingClient = await manager.FindByClientIdAsync(clientId);
        if (existingClient == null)
        {
            await manager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = clientId,
                ClientSecret = clientSecret,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode
                }
            });
        }
        
        var client = _factory.CreateClient();
        
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret)
        });

        // Act
        var response = await client.PostAsync("/connect/token", requestContent);
        
        // Assert
        Assert.False(response.IsSuccessStatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.Contains("error", responseString);
    }
}

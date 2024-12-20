using System.Security.Cryptography.X509Certificates;
using Amazon.Runtime;
using ApogeeDev.IdentityProvider.Host.Data;
using ApogeeDev.IdentityProvider.Host.Initializers;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using ApogeeDev.IdentityProvider.Host.Operations.Processors;
using ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.Abstractions;
using Quartz;
using Serilog;

namespace ApogeeDev.IdentityProvider.Host;

public class Startup
{
    public const string EnvVarPrefix = "APP_";
    private string AppPathPrefix => System.Environment.GetEnvironmentVariable($"{EnvVarPrefix}AppPathPrefix")
        ?? string.Empty;

    public Startup(ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public ConfigurationManager Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        if (!Environment.IsDevelopment())
        {
            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("/dpapi-keys/"));
        }

        var appOptions = new AppOptions();
        Configuration.GetSection(AppOptions.SectionName)
            .Bind(appOptions);

        services.AddSerilog((s, lc) => lc.ReadFrom.Configuration(Configuration));

        services.AddOptions();
        services.Configure<AppOptions>(
                Configuration.GetSection(AppOptions.SectionName));
        services.Configure<AppClientOptions>(
                Configuration.GetSection(AppClientOptions.SectionName));
        services.Configure<OAuthWebProviderOptions>(
                Configuration.GetSection(OAuthWebProviderOptions.SectionName));

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.RequireHeaderSymmetry = false;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        var mvcBuilder = services.AddControllersWithViews();

#if DEBUG
        // Only use Runtime Compilation on Debug
        if (Environment.IsDevelopment())
        {
            mvcBuilder.AddRazorRuntimeCompilation();
        }
#endif

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.RequireHeaderSymmetry = false;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddHealthChecks()
            .AddCheck<DbConnectionHealthCheck>("dbcheck");

        /*services.AddQuartz(options =>
        {
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);*/

        services.AddSingleton<IMongoClient>(new MongoClient(appOptions.MongoDbConnection));
        services.AddSingleton(sp =>
            sp.GetRequiredService<IMongoClient>().GetDatabase(appOptions.DatabaseName)
        );

        ConfigureOpenIdDictServices(services);

        services.AddAuthorization()
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie();

        ConfigureAppServices(services);
    }

    private void ConfigureAppServices(IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
            cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly);
        });

        services.AddTransient<ICryptoHelper, CryptoHelper>();
        services.AddTransient<GithubClaimsProcessor>();
        services.AddTransient<GoogleClaimsProcessor>();

        services.AddDbContext<ApplicationDbContext>(
            (sp, o) => o.UseMongoDB(sp.GetRequiredService<IMongoClient>(),
                sp.GetRequiredService<IOptions<AppOptions>>().Value.DatabaseName)
        );
    }

    private void ConfigureOpenIdDictServices(IServiceCollection services)
    {
        services.AddOpenIddict()
        .AddCore(_ =>
        {
            // uses registered IMongoDatabase from DI
            _.UseMongoDb();
            //_.UseQuartz();
        })
        .AddClient(o => ConfigureOpenIdDictClient(o))
        .AddServer(o => ConfigureOpenIdDictServer(o))
        .AddValidation(o =>
        {
            o.UseLocalServer();
            o.UseAspNetCore();
            //o.UseDataProtection(); // map to docker volume
        });

        services.AddHostedService<MongoDbPerformanceInitializer>();
        services.AddHostedService<ApplicationClientInitializer>();
    }

    private void ConfigureOpenIdDictServer(OpenIddictServerBuilder o)
    {
        o.RegisterScopes(
            OpenIddictConstants.Scopes.Address,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Phone,
            OpenIddictConstants.Scopes.Roles);

        o.SetAuthorizationEndpointUris($"{AppPathPrefix}/connect/authorize")
            .SetTokenEndpointUris($"{AppPathPrefix}/connect/token")
            .SetLogoutEndpointUris($"{AppPathPrefix}/connect/logout")
            .SetUserinfoEndpointUris($"{AppPathPrefix}/connect/userinfo")
            .AllowAuthorizationCodeFlow()
            .RequireProofKeyForCodeExchange()
            .AddEncryptionCertificate(new X509Certificate2(File.ReadAllBytes(Configuration["AppOptions:EncryptionCert"]!)))
            .AddSigningCertificate(new X509Certificate2(File.ReadAllBytes(Configuration["AppOptions:SigningCert"]!)))
            .UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableLogoutEndpointPassthrough()
            //.EnableTokenEndpointPassthrough()
            .EnableUserinfoEndpointPassthrough()
            .DisableTransportSecurityRequirement();
    }

    private void ConfigureOpenIdDictClient(OpenIddictClientBuilder options)
    {
        var webProviders = new OAuthWebProviderOptions();
        Configuration.GetSection(OAuthWebProviderOptions.SectionName)
            .Bind(webProviders);

        options.AllowAuthorizationCodeFlow()
            .AllowRefreshTokenFlow()
            .AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate()
            .UseAspNetCore()
            .EnableStatusCodePagesIntegration()
            .EnableRedirectionEndpointPassthrough();

        options.UseSystemNetHttp()
            .SetProductInformation(typeof(Startup).Assembly);

        options.UseWebProviders()
            .AddGitHub(opts =>
            {
                opts.SetClientId(webProviders.Github.ClientId)
                    .SetClientSecret(webProviders.Github.ClientSecret)
                    .SetRedirectUri(webProviders.Github.RedirectUri)
                    .AddScopes(webProviders.Github.Scopes);
            })
            .AddGoogle(opts =>
            {
                opts.SetClientId(webProviders.Google.ClientId)
                    .SetClientSecret(webProviders.Google.ClientSecret)
                    .SetRedirectUri(webProviders.Google.RedirectUri)
                    .AddScopes(webProviders.Google.Scopes);
            });
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseForwardedHeaders();
        app.UseSerilogRequestLogging();

        if (!string.IsNullOrWhiteSpace(AppPathPrefix))
        {
            app.Use((context, next) =>
            {
                context.Request.PathBase = AppPathPrefix;
                return next();
            });
        }
        // Configure the HTTP request pipeline.
        if (Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseHttpsRedirection();
        }

        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapDefaultControllerRoute();
            endpoints.MapHealthChecks("/healthcheck");
        });
    }
}
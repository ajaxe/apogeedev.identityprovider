using ApogeeDev.IdentityProvider.Host.Initializers;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using MongoDB.Driver;
using Quartz;
using Serilog;

namespace ApogeeDev.IdentityProvider.Host;

public class Startup
{
    public Startup(ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public ConfigurationManager Configuration { get; }
    public IWebHostEnvironment Environment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
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

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddControllersWithViews();
        services.AddHealthChecks();

        services.AddQuartz(options =>
        {
            options.UseSimpleTypeLoader();
            options.UseInMemoryStore();
        });
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

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
            cfg.RegisterServicesFromAssembly(typeof(Startup).Assembly);
        });
        services.AddTransient<ICryptoHelper, CryptoHelper>();
    }

    private void ConfigureOpenIdDictServices(IServiceCollection services)
    {
        services.AddOpenIddict()
        .AddCore(_ =>
        {
            // uses registered IMongoDatabase from DI
            _.UseMongoDb();
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
        o.SetAuthorizationEndpointUris("/connect/authorize")
            .SetTokenEndpointUris("/connect/token")
            .AllowAuthorizationCodeFlow()
            .RequireProofKeyForCodeExchange()
            .AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate()
            .UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough();
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
                    .SetRedirectUri(webProviders.Github.RedirectUri);
            });
        /*.AddGoogle(opts =>
        {
            opts.SetClientId("")
                .SetClientSecret("")
                .SetRedirectUri("callback/login/google");
        });*/
    }

    public void Configure(IApplicationBuilder app)
    {
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

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapDefaultControllerRoute();
            endpoints.MapHealthChecks("/healthcheck");
        });
    }
}
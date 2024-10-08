
using ApogeeDev.IdentityProvider.Host.Data;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.EntityFrameworkCore;
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

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddControllers();

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
    }

    private void ConfigureOpenIdDictServices(IServiceCollection services)
    {
        services.AddOpenIddict()
        .AddCore(options =>
        {
            options.UseMongoDb();
        });

        services.AddHostedService<MongoDbPerformanceInitializer>();
    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure the HTTP request pipeline.
        if (Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/healthcheck");
        });
    }
}
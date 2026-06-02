using ApogeeDev.IdentityProvider.Host;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var secretsFile = Environment.GetEnvironmentVariable("Secrets_File") ?? string.Empty;
var isSecretFileOptional = false;

if (string.IsNullOrWhiteSpace(secretsFile))
{
    secretsFile = "secrets.json";
    isSecretFileOptional = true;
}

builder.Configuration
    // map to docker volume to load client config
    .AddJsonFile(secretsFile, optional: isSecretFileOptional, reloadOnChange: true)
    .AddEnvironmentVariables(prefix: Startup.EnvVarPrefix);

using var loggerFactory = LoggerFactory.Create(logbuilder =>
{
    logbuilder.ClearProviders();
    logbuilder.AddSerilog(new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger(), dispose: true);
});

var startup = new Startup(builder.Configuration, builder.Environment, loggerFactory.CreateLogger<Startup>());

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app);

app.Run();

public partial class Program { }

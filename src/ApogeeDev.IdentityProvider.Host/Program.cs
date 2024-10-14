using ApogeeDev.IdentityProvider.Host;

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

var startup = new Startup(builder.Configuration, builder.Environment);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app);

app.Run();

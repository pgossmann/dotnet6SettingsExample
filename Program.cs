var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.WebHost.UseKestrel(serverOptions => { serverOptions.ListenAnyIP(5050); });

// Build a config object, using env vars and JSON providers.
string envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "unknown";

// check Launch settings
// launch the app using 
// dotnet run --launch-profile Local
// overwrite settings like this
// dotnet run --launch-profile Local
// --Settings:ClientSecret XYZ
Console.WriteLine(envName);
IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{envName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

// the following line configures the IOptions that we can inject everywhere
var configSection = config.GetRequiredSection("Settings");
builder.Services.Configure<Settings>(configSection);

// if we need to use the settings here we can do that with the following line
Settings settings = configSection.Get<Settings>();

// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-7.0
// describes to use Launch settings
var app = builder.Build();

app.MapControllers();

app.Run();

# dotnet6SettingsExample
Working with appsettings.json and appsettings.&lt;envoironment>.json was a mystery to me, so I created an example project and tried out what works for me


## Kestrel
I am using kestrel and I fixed the port in the Program.cs
```builder.WebHost.UseKestrel(serverOptions => { serverOptions.ListenAnyIP(5050); });```
Access the testpage by using http://localhost:5050/settings/get.

## Settings.cs
I created a Settings class with all the settings an app would need. 
It has flat properties but could also have a nested structure.

## How to set settings per environment?
There is a new Settings section in the appsettings.json for which gets mapped to the properties of the Settings.cs.
```
{
...

  "Settings": {
    "ConnectionString": "default value for ConnectionString",
    "ClientId": "default value for ClientId",
    "ClientSecret": "default value for ClientSecret"
  }
}
```
There are environment specific files like: appsettings.Development.json which overwrite the default appsettings.json.
I introduced a new environment called "Local" just to try out a name that is not one of the default Development, Staging, Production.

## LaunchSettings.json
The LaunchSettings set the ASPNETCORE_ENVIRONMENT variable which is needed if we use custom names like "Local".
```
  "profiles": {
    "Default": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Production"
      }
    },
    "Production": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Production"
      }
    },
    "Local": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Local"
      }
    },
    "Development": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
```

## Which settings to use when using CLI dotnet run? (Command line)
with ```dotnet run --launch-profile Local``` we use the LaunchSetting profile Local which sets the ASPNETCORE_ENVIRONMENT to Local
This allows the Program.cs to load the appsettings.Local.json.

Furthermore any setting can be overruled when launching the command line **dotnet run --Settings:ClientSecret XYZ**
 
## Program.cs
```
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
```

## How to use the Settings? How to inject them? 
Use **IOptions<Settings>** like here:
```
    public SettingsController(IOptions<Settings> settings)
    {
        _settings = settings.Value;
    }

```


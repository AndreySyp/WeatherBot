using Microsoft.Extensions.Configuration;
using WeatherBot.Models;

namespace WeatherBot.Infrastructure;

public class Startup
{
    public Startup()
    {
        var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: false);

        IConfiguration config = builder.Build();

        var settings = config.Get<Settings>() ?? throw new NullReferenceException("Failed to read appsettings.json");
        if (settings.WeatherAPI == "" || settings.ConconnectionSQl == "" || settings.BotToken == "")
        {
            throw new NullReferenceException("Settings appsettings.json options are zero");
        }

        Settings = settings;
    }

    public Settings Settings { get; private set; }
}

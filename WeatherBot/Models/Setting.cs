namespace WeatherBot.Models;

public class Settings
{
    public required string BotToken { get; set; }
    public required string WeatherAPI { get; set; }
    public required string ConconnectionSQl { get; set; }
}

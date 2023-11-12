namespace WeatherBot.Models;

public class Notification
{
    public int Id { get; set; }
    public long ChatId { get; set; }
    public TimeSpan Time { get; set; }
}

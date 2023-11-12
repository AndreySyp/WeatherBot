namespace WeatherBot.Models;

public class User
{
    public int Id { get; set; }
    public long ChatId { get; set; }
    public string Units { get; set; }
    public string City { get; set; }
    public User(string city = "london")
    {
        Units = "metric";
        City = city;
    }
}

namespace WeatherBot.Models;

public class WeatherData
{
    public required string Name { get; set; }
    public required Main Main { get; set; }
    public required Wind Wind { get; set; }
    public required Couds Couds { get; set; }
    public required Coord Coord { get; set; }
}

public class Main
{
    public double Temp { get; set; }
    public int Pressure { get; set; }
    public int Humidity { get; set; }
}

public class Wind
{
    public double Speed { get; set; }
}

public class Coord
{
    public double Lon { get; set; }
    public double Lat { get; set; }
}

public class Couds
{
    public double All { get; set; }
}

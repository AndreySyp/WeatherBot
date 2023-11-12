namespace WeatherBot.Models;

public class Units
{
    public string Temp { get; private set; }
    public string Pressure { get; private set; }
    public string Humidity { get; private set; }
    public string Speed { get; private set; }
    public string All { get; private set; }

    public Units(string units)
    {
        switch (units)
        {
            case "metric":
                Pressure = "hPa";
                Humidity = "%";
                Speed = "m/s";
                Temp = "℃";
                All = "%";
                break;
            case "imperial":

                Pressure = "hPa";
                Humidity = "%";
                Speed = "mph";
                Temp = "°F";
                All = "%";
                break;
            default:
                Pressure = "hPa";
                Humidity = "%";
                Speed = "m/s";
                Temp = "K";
                All = "%";
                break;
        }
    }
}

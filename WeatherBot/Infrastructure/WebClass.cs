using Newtonsoft.Json;
using System.Net;
using WeatherBot.Models;

namespace WeatherBot.Infrastructure;

public class WebClass
{
    private static string? weatherAPI;

    public static string WeatherAPI
    {
        get => weatherAPI ?? throw new NullReferenceException("Failed to connect to API. WeatherAPI equals null");
        set => weatherAPI = value;
    }

    public static WeatherData GetWeatherData(User user, string city)
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?q={city ?? user.City}&appid={WeatherAPI}&units={user.Units}";
        string response;

        HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        HttpWebResponse httpWebResponse;

        try
        {
            httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        }
        catch (Exception)
        {
            throw new Exception("Invalid city");
        }

        using (StreamReader streamReader = new(httpWebResponse.GetResponseStream()))
        {
            response = streamReader.ReadToEnd();
        }

        return JsonConvert.DeserializeObject<WeatherData>(response) ?? throw new Exception("Parsing error");
    }
}

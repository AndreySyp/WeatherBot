using Telegram.Bot.Types;
using Telegram.Bot;
using WeatherBot.Models;
using User = WeatherBot.Models.User;

namespace WeatherBot.Infrastructure;

internal class Commands
{
    public static async Task TimeAddAsync(Message message, User user, HashSet<Notification> notifications, ITelegramBotClient botClient)
    {
        if (!TryPaserParametr(message, out string? time))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Error: Too few parameters");
            return;
        }

        if (!TimeSpan.TryParse(time, out TimeSpan dateAdd))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Error: The time was entered incorrectly");
            return;
        }

        Notification notification = SQLWorker.AddNotification(message, notifications, dateAdd);
        TelegramScheduler.Start(notification, user, botClient);

        await botClient.SendTextMessageAsync(message.Chat.Id, $"A reminder has been added to {dateAdd:t}");
    }
    public static async Task TimeDeleteAsync(Message message, HashSet<Notification> notifications, ITelegramBotClient botClient)
    {
        if (!TryPaserParametr(message, out string? time))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Error: Too few parameters");
            return;
        }

        if (!TimeSpan.TryParse(time, out TimeSpan dateDelete))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Error: The time was entered incorrectly");
            return;
        }

        var t = notifications.FirstOrDefault(x => x.ChatId == message.Chat.Id && dateDelete == x.Time);
        if (t == null)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, $"This time was not found");
            return;
        }

        SQLWorker.DeleteNotification(notifications, t);
        TelegramScheduler.Delete(t);
        await botClient.SendTextMessageAsync(message.Chat.Id, $"The reminder has been removed {dateDelete:t}");
    }

    public static async Task UnitsAsync(Message message, User user, ITelegramBotClient botClient)
    {
        if (!TryPaserParametr(message, out string? unit))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Error: Too few parameters");
            return;
        }

        switch (unit[0])
        {
            case 'm':
                user.Units = "metric";
                break;
            case 'i':
                user.Units = "imperial";
                break;
            case 's':
                user.Units = "standart";
                break;
        }

        SQLWorker.ChangeUser(user);
        await botClient.SendTextMessageAsync(message.Chat.Id, $"Measurement system changed to {user.Units}");
    }
    public static async Task CityAsync(Message message, User user, ITelegramBotClient botClient)
    {
        if (!TryPaserParametr(message, out string? city))
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, $"Error: Too few parameters");
            return;
        }

        user.City = city;
        SQLWorker.ChangeUser(user);
        await botClient.SendTextMessageAsync(message.Chat.Id, $"The city was changed to {city}");
    }

    public static async Task WeatherAsync(Message message, User user, ITelegramBotClient botClient)
    {
        TryPaserParametr(message, out string? city);
        await SendMessage(user, botClient, city);
    }
    public static async Task DefaultAsync(Message message, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Unknown command");
    }
    public static async Task HelpAsync(Message message, ITelegramBotClient botClient)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id,
                                            $"/timeAdd {{time}} - " +
                                            $"/timeDelete {{time}} - " +
                                            $"/units {{unit}}- " +
                                            $"/weather {{city/null}} - ");
    }

    public static async Task SendMessage(User user, ITelegramBotClient botClient, string? city = null)
    {
        try
        {
            var data = WebClass.GetWeatherData(user, city ?? user.City);
            await botClient.SendTextMessageAsync(user.ChatId, Output(data, user.Units));
        }
        catch (NullReferenceException e)
        {
            await botClient.SendTextMessageAsync(user.ChatId, "The program crashed");
            throw e;
        }
        catch (Exception)
        {
            await botClient.SendTextMessageAsync(user.ChatId, "Error: Invalid city");
        }
    }

    private static string Output(WeatherData data, string unitType)
    {
        Units unit = new(unitType);
        return $"In {data.Name}\n" +
                $"temperature {(int)data.Main.Temp} {unit.Temp}\n" +
                $"pressure {data.Main.Pressure} {unit.Pressure}\n" +
                $"humidity {data.Main.Humidity} {unit.Humidity}\n" +
                $"wind speed {(int)data.Wind.Speed} {unit.Speed}";
    }

    private static bool TryPaserParametr(Message message, out string? parameter)
    {
        try
        {
            parameter = message.Text.Split(' ')[1];
            return true;
        }
        catch
        {
            parameter = null;
            return false;
        }
    }
}

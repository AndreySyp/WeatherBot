using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using WeatherBot.Infrastructure;
using WeatherBot.Models;
using User = WeatherBot.Models.User;

namespace WeatherBot.Handles;

public class HandleTelegramBot
{
    private HashSet<Notification> notifications = new();
    private HashSet<User> users = new();

    public HandleTelegramBot()
    {
        SQLWorker.GetData(out notifications, out users);
    }

    public static Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var message = update.Message;

        if (message == null) { return; }
        if (message.Text == null) { return; }

        var user = SQLWorker.AddUser(message, users);

        switch (message.Text.Split(' ')[0])
        {
            case "/help":
                await Commands.HelpAsync(message, botClient);
                break;
            case "/timeAdd":
                await Commands.TimeAddAsync(message, user, notifications, botClient);
                break;
            case "/timeDelete":
                await Commands.TimeDeleteAsync(message, notifications, botClient);
                break;
            case "/city":
                await Commands.CityAsync(message, user, botClient);
                break;
            case "/weather":
                await Commands.WeatherAsync(message, user, botClient);
                break;
            case "/units":
                await Commands.UnitsAsync(message, user, botClient);
                break;
            default:
                await Commands.DefaultAsync(message, botClient);
                break;
        }
    }
}

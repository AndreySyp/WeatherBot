using Telegram.Bot;
using WeatherBot.Handles;
using WeatherBot.Infrastructure;
using WeatherBot.Models;

namespace WeatherBot;

internal class Program
{
    private static void Main()
    {
        Settings settings = new Startup().Settings;

        SQLWorker.ConconnectionSQl = settings.ConconnectionSQl;
        WebClass.WeatherAPI = settings.WeatherAPI;

        TelegramBotClient botClient = new(settings.BotToken);
        HandleTelegramBot handleTelegramBot = new();

        botClient.StartReceiving(handleTelegramBot.HandleUpdateAsync, HandleTelegramBot.HandleErrorAsync);

        SQLWorker.GetData(out HashSet<Notification> notifications, out HashSet<User> users);
        StartMessage(botClient, notifications, users);

        Console.WriteLine("Started");
        Console.ReadLine();
    }

    private static void StartMessage(TelegramBotClient botClient, HashSet<Notification> notifications, HashSet<User> users)
    {
        foreach (var (user, notification) in from user in users
                                             let notificationsUser = notifications.Where(x => x.ChatId == user.ChatId).ToList()
                                             from notification in notificationsUser
                                             select (user, notification))
        {
            TelegramScheduler.Start(notification, user, botClient);
        }
    }
}

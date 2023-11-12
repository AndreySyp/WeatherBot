using Telegram.Bot.Types;
using WeatherBot.Models;
using User = WeatherBot.Models.User;

namespace WeatherBot.Infrastructure;

public class SQLWorker
{
    private static string? conconnectionSQl;

    public static string ConconnectionSQl
    {
        get => conconnectionSQl ?? throw new NullReferenceException("Failed to connect to database. ConconnectionSQl equals null");
        set => conconnectionSQl = value;
    }

    public static void GetData(out HashSet<Notification> notifications, out HashSet<User> users)
    {
        using ApplicationContext db = new(ConconnectionSQl);
        notifications = db.Notifications.ToHashSet();
        users = db.Users.ToHashSet();
    }

    public static User AddUser(Message message, HashSet<User> users)
    {
        User user = new();
        User? temp = users.FirstOrDefault(x => message.Chat.Id == x.ChatId);

        if (temp == null)
        {
            user.ChatId = message.Chat.Id;
            users.Add(user);

            using ApplicationContext db = new(ConconnectionSQl);
            db.Users.AddRange(user);
            db.SaveChanges();
        }
        else
        {
            user = temp;
        }

        return user;
    }
    public static void ChangeUser(User user)
    {
        using ApplicationContext db = new(ConconnectionSQl);
        User userDB = db.Users.First(x => x.ChatId == user.ChatId);
        userDB.Units = user.Units;
        userDB.City = user.City;
        db.SaveChanges();
    }

    public static Notification AddNotification(Message message, HashSet<Notification> notifications, TimeSpan dateAdd)
    {
        Notification notification = new() { ChatId = message.Chat.Id, Time = dateAdd };
        notifications.Add(notification);

        using ApplicationContext db = new(ConconnectionSQl);
        db.Notifications.AddRange(notification);
        db.SaveChanges();

        return notification;
    }
    public static void DeleteNotification(HashSet<Notification> notifications, Notification notification)
    {
        notifications.Remove(notification);

        using ApplicationContext db = new(ConconnectionSQl);
        db.Notifications.Remove(notification);
        db.SaveChanges();
    }
}

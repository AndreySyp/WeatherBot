using Quartz;
using Quartz.Impl;
using Telegram.Bot;
using WeatherBot.Models;

namespace WeatherBot.Infrastructure;

public class TelegramScheduler
{
    public static async void Start(Notification notification, User user, ITelegramBotClient botClient)
    {
        var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
        await scheduler.Start();

        JobKey jobKey = new(notification.Time.ToString(), notification.ChatId.ToString());
        var job = JobBuilder.Create<TelegramSender>().WithIdentity(jobKey).Build();
        var dateTime = TimeChoice(notification);

        ITrigger trigger = TriggerBuilder.Create()
            .WithIdentity(notification.Time.ToString(), notification.ChatId.ToString())
            .StartAt(dateTime)
            .WithSimpleSchedule(x => x
                .WithIntervalInHours(24)
                .RepeatForever())
            .Build();

        job.JobDataMap["user"] = user;
        job.JobDataMap["botClient"] = botClient;

        await scheduler.ScheduleJob(job, trigger);
    }

    public static async void Delete(Notification notification)
    {

        JobKey jobKey = new(notification.Time.ToString(), notification.ChatId.ToString());

        var scheduler = StdSchedulerFactory.GetDefaultScheduler().Result;
        var executingJobs = scheduler.GetCurrentlyExecutingJobs().Result;

        if (executingJobs.Any(x => x.JobDetail.Key.Equals(jobKey)))
        {
            await scheduler.Interrupt(jobKey);
        }

        await scheduler.DeleteJob(jobKey);
    }

    public static DateTime TimeChoice(Notification notification)
    {
        DateTime now = DateTime.Now;
        DateTime dateTime = new(now.Year, now.Month, now.Day,
                                notification.Time.Hours, notification.Time.Minutes, notification.Time.Seconds);

        if (notification.Time < now.TimeOfDay) { dateTime.AddDays(1); }

        return dateTime;
    }
}

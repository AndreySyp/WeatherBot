using Quartz;
using Telegram.Bot;
using WeatherBot.Models;

namespace WeatherBot.Infrastructure;

public class TelegramSender : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        if (context.JobDetail.JobDataMap["user"] is User user
            && context.JobDetail.JobDataMap["botClient"] is ITelegramBotClient botClient)
        {
            await Commands.SendMessage(user, botClient);
        }
    }
}

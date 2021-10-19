using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using UdemyFreeCoursesBot.Models;

namespace UdemyFreeCoursesBot.Func
{
    public class NotificationsService
    {

        public async Task SendNotifications(List<UdemyCourse> courses)
        {
            string tableName = "UdemyCourses";

            var tableClient = new TableClient(
                Environment.GetEnvironmentVariable("TableConnection"),
                tableName);

            foreach (var item in courses)
            {
                var queryResultsFilter = tableClient.Query<TableEntity>(filter: $"Url eq '{item.Url}'");

                if (queryResultsFilter.Count() == 0)
                {
                    var entity = new TableEntity("courses", Guid.NewGuid().ToString())
                    {
                        { "Url", item.Url }
                    };

                    tableClient.AddEntity(entity);

                    try
                    {
                        var botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("BotToken"));

                        await botClient.SendTextMessageAsync(
                            chatId: "@udemycouponsfreecourses",
                            text: @$"*{item.Title}* {item.Description} {item.Url}",
                            parseMode: ParseMode.Markdown
                        );
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

    }
}

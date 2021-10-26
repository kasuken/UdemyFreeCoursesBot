using Azure.Data.Tables;
using FaunaDB.Client;
using FaunaDB.Types;
using FaunaDB.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using UdemyFreeCoursesBot.Models;

using static FaunaDB.Query.Language;

namespace UdemyFreeCoursesBot.Func
{
    public class NotificationsServiceFauna
    {
        static readonly string ENDPOINT = Environment.GetEnvironmentVariable("FaunaRegion");
        static readonly string SECRET = Environment.GetEnvironmentVariable("FaunaKey");

        public async Task SendNotifications(List<UdemyCourse> courses)
        {
            var client = new FaunaClient(endpoint: ENDPOINT, secret: SECRET);

            foreach (var item in courses)
            {
                try
                {
                    var result = await client.Query(
                            Get(
                                Match(
                                    Index("courses_urls"),
                                    item.Url
                                )
                            )
                        );

                    continue;
                }
                catch (Exception ex)
                {
                    await client.Query(
                        Create(
                            Collection("courses"),
                            Obj("data", FaunaDB.Types.Encoder.Encode(item))
                        )
                    );

                    try
                    {
                        var botClient = new TelegramBotClient(Environment.GetEnvironmentVariable("BotToken"));

                        await botClient.SendTextMessageAsync(
                            chatId: "@udemycouponsfreecourses",
                            text: @$"*{item.Title}* {item.Description} {item.Url}",
                            parseMode: ParseMode.Markdown
                        );
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}

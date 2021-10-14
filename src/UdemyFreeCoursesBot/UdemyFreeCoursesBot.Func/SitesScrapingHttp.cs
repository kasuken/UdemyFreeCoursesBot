using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Azure;
using Azure.Data.Tables;
using Azure.Data.Tables.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using UdemyFreeCoursesBot.Models;

using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace UdemyFreeCoursesBot.Func
{
    public static class SitesScrapingHttp
    {
        private static List<UdemyCourse> Courses { get; set; }

        [Function("SitesScrapingHttp")]
        public async static Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req,
            FunctionContext executionContext)
        {
            var logger = executionContext.GetLogger("SitesScrapingHttp");
            logger.LogInformation("SitesScrapingHttp processed a request.");

            var engineDiscudemy = new ScrapingEngineDiscudemy();

            Courses = new List<UdemyCourse>();
            Courses.AddRange(await engineDiscudemy.Run());

            var response = req.CreateResponse(HttpStatusCode.OK);

            await response.WriteAsJsonAsync(Courses);

            string tableName = "UdemyCourses";

            var tableClient = new TableClient(
                Environment.GetEnvironmentVariable("TableConnection"),
                tableName);

            foreach (var item in Courses)
            {
                var queryResultsFilter = tableClient.Query<TableEntity>(filter: $"Url eq '{item.Url}'");

                if (queryResultsFilter.Count() == 0)
                {
                    var entity = new TableEntity("courses", Guid.NewGuid().ToString())
                    {
                        { "Url", item.Url }
                    };

                    tableClient.AddEntity(entity);

                    var botClient = new TelegramBotClient("");

                    await botClient.SendTextMessageAsync(
                        chatId: "@udemycouponsfreecourses",
                        text: @$"*{item.Title}* {item.Description} {item.Url}",
                        parseMode: ParseMode.Markdown
                    );

                    //await botClient.SendPhotoAsync()
                }
            }

            return response;
        }
    }
}

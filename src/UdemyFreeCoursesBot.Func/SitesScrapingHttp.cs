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
            var notificationService = new NotificationsServiceFauna();

            Courses = new List<UdemyCourse>();
            Courses.AddRange(await engineDiscudemy.Run());

            await notificationService.SendNotifications(Courses);

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(Courses);

            return response;
        }
    }
}

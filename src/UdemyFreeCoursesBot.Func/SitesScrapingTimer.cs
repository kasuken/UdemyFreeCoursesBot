using System;
using System.Collections.Generic;
using System.Linq;
using Azure.Data.Tables;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using UdemyFreeCoursesBot.Models;

namespace UdemyFreeCoursesBot.Func
{
    public static class SitesScrapingTimer
    {
        private static List<UdemyCourse> Courses { get; set; }

        [Function("SitesScrapingTimer")]
        public async static void Run([TimerTrigger("0 0 * * * *")] MyInfo myTimer, FunctionContext context)
        {
            var logger = context.GetLogger("SitesScrapingTimer");
            logger.LogInformation($"SitesScrapingTimer executed at: {DateTime.Now}");

            Courses = new List<UdemyCourse>();

            var engineDiscudemy = new ScrapingEngineDiscudemy();
            var notificationService = new NotificationsService();

            Courses = new List<UdemyCourse>();
            Courses.AddRange(await engineDiscudemy.Run());

            await notificationService.SendNotifications(Courses);

            logger.LogInformation($"Next timer schedule at: {myTimer.ScheduleStatus.Next}");
        }
    }

    public class MyInfo
    {
        public MyScheduleStatus ScheduleStatus { get; set; }

        public bool IsPastDue { get; set; }
    }

    public class MyScheduleStatus
    {
        public DateTime Last { get; set; }

        public DateTime Next { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}

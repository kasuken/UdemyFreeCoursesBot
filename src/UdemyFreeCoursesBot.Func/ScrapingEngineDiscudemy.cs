using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UdemyFreeCoursesBot.Models;

namespace UdemyFreeCoursesBot.Func
{
    public class ScrapingEngineDiscudemy
    {

        public async Task<List<UdemyCourse>> Run()
        {
            var courses = new List<UdemyCourse>();

            var url = "https://www.discudemy.com/all";

            var cancellationToken = new CancellationTokenSource();
            var httpClient = new HttpClient();
            var request = await httpClient.GetAsync(url);
            cancellationToken.Token.ThrowIfCancellationRequested();

            var responseStream = await request.Content.ReadAsStreamAsync();
            cancellationToken.Token.ThrowIfCancellationRequested();

            var parser = new HtmlParser();
            var document = parser.ParseDocument(responseStream);

            var cards = document.GetElementsByClassName("card");

            foreach (var card in cards)
            {
                try
                {
                    var courseLink = card.GetElementsByClassName("card-header");
                    var courseTitle = courseLink[0]?.TextContent;
                    var courseDescription = card.GetElementsByClassName("description")[0]?.TextContent;

                    courseDescription = courseDescription.Replace("\n                ", "\n");
                    courseDescription = courseDescription.Replace(" \n\t\t\t\t\n            ", "\n");
                    
                    var courseBanner = card.GetElementsByClassName("ui full-width image")[0]?.GetAttribute("src");
                    var courseUrl = courseLink[0].GetAttribute("href");

                    request = await httpClient.GetAsync(courseUrl);
                    cancellationToken.Token.ThrowIfCancellationRequested();

                    responseStream = await request.Content.ReadAsStreamAsync();
                    cancellationToken.Token.ThrowIfCancellationRequested();

                    parser = new HtmlParser();
                    document = parser.ParseDocument(responseStream);

                    var discBtn = document.GetElementsByClassName("discBtn")[0];
                    courseUrl = discBtn?.GetAttribute("href");

                    request = await httpClient.GetAsync(courseUrl);
                    cancellationToken.Token.ThrowIfCancellationRequested();

                    responseStream = await request.Content.ReadAsStreamAsync();
                    cancellationToken.Token.ThrowIfCancellationRequested();

                    parser = new HtmlParser();
                    document = parser.ParseDocument(responseStream);

                    var container = document.GetElementsByClassName("ui segment")[0];
                    var a = container?.GetElementsByTagName("a")[0];

                    courseUrl = a.GetAttribute("href");

                    courses.Add(new UdemyCourse() { Title = courseTitle, Description = courseDescription, BannerUrl = courseBanner, Url = courseUrl });
                }
                catch (System.Exception ex)
                {
                    //logger.LogInformation(ex.Message);
                    continue;
                }
            }

            return courses;
        }
    }
}

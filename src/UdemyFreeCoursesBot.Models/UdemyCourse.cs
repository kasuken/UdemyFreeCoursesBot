using FaunaDB.Types;
using System;

namespace UdemyFreeCoursesBot.Models
{
    public class UdemyCourse
    {
        [FaunaField("title")]
        public string Title { get; set; }

        [FaunaField("description")]
        public string Description { get; set; }

        [FaunaField("bannerurl")]
        public string BannerUrl { get; set; }

        [FaunaField("url")]
        public string Url { get; set; }
    }

}

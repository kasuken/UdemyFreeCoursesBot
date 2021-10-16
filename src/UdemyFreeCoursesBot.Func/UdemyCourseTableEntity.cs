
using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyFreeCoursesBot.Func
{
    public class UdemyCourseTableEntity : TableEntity
    {

        public string Url { get; set; }

    }
}

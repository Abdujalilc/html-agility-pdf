using HtmlAgilityPack;
using System.Xml;

namespace ChangeSpanDateFormat.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToCustomDateString(this DateTime dateTime, string dateFormatTemplate)
        {
            // Convert the newValue to match the format of the template
            string formattedDate = dateTime.ToString(dateFormatTemplate);
            return formattedDate;
        }
    }
}

using System;
using System.Globalization;

namespace TicDrive.Utils.DateTime
{
    public static class DateTimeFormatter
    {
        public static string FormatLocalDateInItalian(System.DateTime localDate)
        {
            CultureInfo culture = new CultureInfo("it-IT");

            string dayOfWeek = culture.DateTimeFormat.GetDayName(localDate.DayOfWeek);
            string monthName = culture.DateTimeFormat.GetMonthName(localDate.Month);

            dayOfWeek = char.ToUpper(dayOfWeek[0]) + dayOfWeek[1..];
            monthName = char.ToUpper(monthName[0]) + monthName[1..];

            return $"{dayOfWeek} {localDate.Day} {monthName} - {localDate:HH:mm}";
        }
    }
}

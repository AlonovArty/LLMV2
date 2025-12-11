using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigaChatv2.Classes
{
    public static class HolidayCalendar
    {
        public static Dictionary<DateTime, string> Holidays = new Dictionary<DateTime, string>()
        {
            { new DateTime(DateTime.Now.Year, 1, 1), "Новый год" },
            { new DateTime(DateTime.Now.Year, 2, 14), "День святого Валентина" },
            { new DateTime(DateTime.Now.Year, 2, 23), "День защитника Отечества" },
            { new DateTime(DateTime.Now.Year, 3, 8), "Международный женский день" },
            { new DateTime(DateTime.Now.Year, 5, 9), "День Победы" },
            { new DateTime(DateTime.Now.Year, 6, 1), "День защиты детей" },
            { new DateTime(DateTime.Now.Year, 12, 31), "Новогодняя ночь" }
        };

        public static string GetNearestHoliday()
        {
            DateTime today = DateTime.Today;
            DateTime nearestDate = DateTime.MaxValue;
            string nearestHoliday = "неизвестный праздник";

            foreach (var h in Holidays)
            {
                DateTime d = h.Key;
                if (d >= today && d < nearestDate)
                {
                    nearestDate = d;
                    nearestHoliday = h.Value;
                }
            }

            return nearestHoliday;
        }
    }
}

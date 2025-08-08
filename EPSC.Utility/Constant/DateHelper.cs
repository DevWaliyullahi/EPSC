namespace EPSC.Utility.Constant
{
    public static class DateHelper
    {
        public static string ToCustomDateString(this DateTime d)
        {
            return d.ToString("dd-MM-yyyy hh:mm tt");
        }

        public static string ToHumanReadableDate(this DateTime date)
        {
            TimeSpan timeDifference = DateTime.UtcNow - date;

            if (timeDifference.TotalSeconds < 60)
                return "now";
            if (timeDifference.TotalMinutes < 60)
                return $"{(int)timeDifference.TotalMinutes} minute(s) ago";
            if (timeDifference.TotalHours < 24)
                return $"{(int)timeDifference.TotalHours} hour(s) ago";
            if (timeDifference.TotalDays < 7)
                return $"{(int)timeDifference.TotalDays} day(s) ago";
            if (timeDifference.TotalDays < 30)
                return $"{(int)(timeDifference.TotalDays / 7)} week(s) ago";
            if (timeDifference.TotalDays < 365)
                return "over a month ago";

            int years = (int)(timeDifference.TotalDays / 365);
            return years == 1 ? "1 year ago" : $"{years} years ago";
        }
    }
}

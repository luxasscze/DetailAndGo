namespace DetailAndGoAdmin.UtilityStatic
{
    public static class DateAndTime
    {
        public static string GetDaysToDate(DateTime date)
        {
            DateTime now = DateTime.Now;
            double remainingDays = (date.Date - now.Date).TotalDays;

            if(remainingDays < 1 && remainingDays >= 0)
            {
                return "today";
            }
            else if(remainingDays >= 0 && remainingDays < 2)
            {
                return "tomorrow";
            }
            else if(remainingDays < 0)
            {
                return "MISSED BOOKING!";
            }
            return "in " + remainingDays.ToString() + " days";
        }

        public static string GetDaysFromDate(DateTime date)
        {
            DateTime now = DateTime.Now;
            double daysFrom = (now.Date - date.Date).TotalDays;

            if(daysFrom < 1)
            {
                return "today";
            }
            else if (daysFrom >= 1 && daysFrom < 2)
            {
                return "yesterday";
            }

            return daysFrom.ToString() + " days ago";
        }
    }
}

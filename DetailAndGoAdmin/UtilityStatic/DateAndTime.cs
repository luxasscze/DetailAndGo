namespace DetailAndGoAdmin.UtilityStatic
{
    public static class DateAndTime
    {
        public static string GetDaysToDate(DateTime date)
        {
            DateTime now = DateTime.Now;
            double remainingDays = Math.Round((date - now).TotalDays, 0);

            if(remainingDays == 1)
            {
                return "tomorrow";
            }
            else if(remainingDays == 0)
            {
                return "today";
            }
            return "in " + remainingDays.ToString() + " days";
        }

        public static string GetDaysFromDate(DateTime date)
        {
            DateTime now = DateTime.Now;
            double daysFrom = Math.Round((now - date).TotalDays, 0);

            if(daysFrom == 0)
            {
                return "today";
            }
            else if (daysFrom == 1)
            {
                return "yesterday";
            }

            return daysFrom.ToString() + " days ago";
        }
    }
}

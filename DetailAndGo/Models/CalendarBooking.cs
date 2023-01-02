namespace DetailAndGo.Models
{
    public class CalendarBooking
    {
        public string title { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public bool allDay { get; set; }
        public string id { get; set; }
        public string groupId { get; set; }
        public string url { get; set; }
        public string[] classNames { get; set; }
        public bool editable { get; set; }
        public string backgroundColor { get; set; }
        public string borderColor { get; set; }
        public string textColor { get; set; }
        public Dictionary<string, string> extendedProps { get; set; }
        public string display { get; set; }

    }
}

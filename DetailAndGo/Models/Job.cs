using Microsoft.CodeAnalysis;

namespace DetailAndGo.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string CustomerId { get; set; }        
        public string Services { get; set; }
        public DateTime Created { get; set; }
        public DateTime WhenAccepted { get; set; }
        public DateTime WhenFinished { get; set; }
        public DateTime Updated { get; set; }
        public double LocationLat { get; set; }
        public double LocationLon { get; set; }
        public string CustomerNotes { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }
}

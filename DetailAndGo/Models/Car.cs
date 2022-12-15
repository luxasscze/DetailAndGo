namespace DetailAndGo.Models
{
    public class Car
    {
        public int Id { get; set; }
        public string AspNetUserId { get; set; }
        public string CarModel { get; set; }
        public string CarFamily { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime Created { get; set; }
        public string Notes { get; set; }
    }
}

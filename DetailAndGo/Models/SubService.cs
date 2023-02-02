namespace DetailAndGo.Models
{
    public class SubService
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string? ForServices { get; set; }
    }
}

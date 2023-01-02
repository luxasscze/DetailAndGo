namespace DetailAndGo.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal PriceMedium { get; set; }
        public decimal PriceLarge { get; set; }
        public decimal Price4x4 { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Image { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }
        public string StripeServiceId { get; set; }

    }
}

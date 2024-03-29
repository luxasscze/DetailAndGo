﻿using DetailAndGo.Migrations;

namespace DetailAndGo.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string AspNetUserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Registered { get; set; }
        public string Email { get; set; }
        public string PostCode { get; set; }
        public string Address1 { get; set; }
        public string? Address2 { get; set; }
        public string? Address3 { get; set; }
        public string? Notes { get; set; }
        public string? StripeId { get; set; }
        public string? CarModel { get; set; }
        public string? CarFamily { get; set; }
        public string? CarSize { get; set; }
        public string? PhoneNumber { get; set; }

    }
}

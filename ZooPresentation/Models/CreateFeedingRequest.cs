using System;
namespace ZooPresentation.Models
{
    public class CreateFeedingRequest
    {
        public Guid AnimalId { get; set; }
        public DateTime FeedingTime { get; set; }
        public string FoodType { get; set; } = default!;
    }
}


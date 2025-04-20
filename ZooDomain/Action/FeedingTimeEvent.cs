using System;
namespace ZooDomain.Action
{
    /// <summary>
    /// Событие выполнения кормления.
    /// </summary>
    public sealed record FeedingTimeEvent(
        Guid AnimalId,
        DateTime ScheduledAt,
        string FoodType,
        DateTime OccurredAt
    ) : IDomainEvent
    {
        public FeedingTimeEvent(Guid animalId, DateTime scheduledAt, string foodType)
            : this(animalId, scheduledAt, foodType, DateTime.UtcNow) { }
    }
}


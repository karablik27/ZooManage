using System;
namespace ZooApplication.DTOs
{
    public record FeedingScheduleDto(
        Guid Id,
        Guid AnimalId,
        DateTime FeedingTime,
        string FoodType,
        bool IsCompleted
    );
}


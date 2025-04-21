// File: ZooTests/Domain/FeedingScheduleTests.cs
using System;
using Xunit;
using ZooDomain.Entities;
using ZooDomain.Action;
using System.Linq;
using ZooDomain;

namespace ZooTests.Domain
{
    public class FeedingScheduleTests
    {
        [Fact]
        public void MarkCompleted_FirstTime_SetsFlagAndAddsEvent()
        {
            // Arrange
            var schedule = new FeedingSchedule(
                Guid.NewGuid(),
                DateTime.UtcNow.AddHours(1),
                "Hay"
            );

            // Act
            schedule.MarkCompleted();

            // Assert: флаг выполнен
            Assert.True(schedule.IsCompleted, "После первого MarkCompleted() IsCompleted должен стать true");

            // Assert: одно событие FeedingTimeEvent
            Assert.Single(schedule.DomainEvents.OfType<FeedingTimeEvent>());
        }

        [Fact]
        public void MarkCompleted_Twice_ThrowsDomainException()
        {
            // Arrange
            var schedule = new FeedingSchedule(
                Guid.NewGuid(),
                DateTime.UtcNow.AddHours(2),
                "Hay"
            );
            schedule.MarkCompleted();

            // Act & Assert: второй вызов бросает
            var ex = Assert.Throws<DomainException>(() => schedule.MarkCompleted());
            Assert.Equal("Кормление уже отмечено.", ex.Message);
        }
    }
}

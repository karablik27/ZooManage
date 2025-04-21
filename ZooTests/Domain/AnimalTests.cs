// File: ZooTests/Domain/AnimalTests.cs
using System;
using System.Linq;
using Xunit;
using ZooDomain.Entities;
using ZooDomain.Action;
using ZooDomain.Enums;
using ZooDomain;

namespace ZooTests.Domain
{
    public class AnimalTests
    {
        [Fact]
        public void MoveTo_SameEnclosure_ThrowsDomainException()
        {
            // Arrange
            var animal = new Animal(
                species: "Lion",
                name: "Leo",
                dateOfBirth: DateTime.UtcNow.AddYears(-3),
                gender: Gender.Male,
                favoriteFood: new Food("Meat", 200)
            );
            var enclosureId = Guid.NewGuid();
            animal.MoveTo(enclosureId);

            // Act & Assert
            var ex = Assert.Throws<DomainException>(() => animal.MoveTo(enclosureId));
            Assert.Equal("Животное уже в этом вольере.", ex.Message);
        }

        [Fact]
        public void MoveTo_NewEnclosure_AddsDomainEvent()
        {
            // Arrange
            var animal = new Animal(
                "Tiger", "Tiggy",
                DateTime.UtcNow.AddYears(-2),
                Gender.Female,
                new Food("Meat", 150)
            );
            var oldId = Guid.Empty;
            var newId = Guid.NewGuid();

            // Act
            animal.MoveTo(newId);

            // Assert EnclosureId
            Assert.Equal(newId, animal.EnclosureId);

            // Assert exactly one event of type AnimalMovedEvent
            var events = animal.DomainEvents.ToList();
            var movedEvents = events.OfType<AnimalMovedEvent>().ToList();
            Assert.Single(movedEvents);

            var evt = movedEvents[0];
            Assert.Equal(animal.Id, evt.AnimalId);
            Assert.Equal(oldId, evt.FromEnclosureId);
            Assert.Equal(newId, evt.ToEnclosureId);
            // Время должно быть близко к сейчас (±1 секунда)
            var diff = (DateTime.UtcNow - evt.OccurredAt).Duration();
            Assert.True(diff <= TimeSpan.FromSeconds(1),
                        $"OccurredAt должен быть близок к DateTime.UtcNow, разница была {diff.TotalSeconds} сек.");
        }
    }
}

// File: ZooTests/Application/ZooStatisticsServiceTests.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ZooApplication.Services;
using ZooApplication.Interfaces.Repositories;
using ZooApplication.DTOs;
using ZooDomain.Entities;
using ZooDomain.Enums;
using ZooDomain.Action;
using Zoo.Domain.Entities;
using ZooDomain;

namespace ZooTests.Application
{
    public class ZooStatisticsServiceTests
    {
        // Stub для IAnimalRepository
        private class StubAnimalRepo : IAnimalRepository
        {
            public Func<Task<IReadOnlyCollection<Animal>>> GetAllAsyncImpl { get; set; }
                = () => Task.FromResult((IReadOnlyCollection<Animal>)Array.Empty<Animal>());

            public Task<IReadOnlyCollection<Animal>> GetAllAsync() => GetAllAsyncImpl();
            public Task<Animal?> GetByIdAsync(Guid id) => Task.FromResult<Animal?>(null!);
            public Task AddAsync(Animal entity) => Task.CompletedTask;
            public Task RemoveAsync(Animal entity) => Task.CompletedTask;
            public Task SaveChangesAsync() => Task.CompletedTask;
        }

        // Stub для IEnclosureRepository
        private class StubEnclosureRepo : IEnclosureRepository
        {
            public Func<Task<IReadOnlyCollection<Enclosure>>> GetAllAsyncImpl { get; set; }
                = () => Task.FromResult((IReadOnlyCollection<Enclosure>)Array.Empty<Enclosure>());

            public Task<IReadOnlyCollection<Enclosure>> GetAllAsync() => GetAllAsyncImpl();
            public Task<Enclosure?> GetByIdAsync(Guid id) => Task.FromResult<Enclosure?>(null!);
            public Task AddAsync(Enclosure entity) => Task.CompletedTask;
            public Task RemoveAsync(Enclosure entity) => Task.CompletedTask;
            public Task SaveChangesAsync() => Task.CompletedTask;
        }

        // Stub для IFeedingScheduleRepository
        private class StubScheduleRepo : IFeedingScheduleRepository
        {
            public Func<Guid, Task<IReadOnlyCollection<FeedingSchedule>>> GetByAnimalAsyncImpl { get; set; }
                = _ => Task.FromResult((IReadOnlyCollection<FeedingSchedule>)Array.Empty<FeedingSchedule>());

            public Task<IReadOnlyCollection<FeedingSchedule>> GetByAnimalAsync(Guid animalId)
                => GetByAnimalAsyncImpl(animalId);

            public Task<FeedingSchedule?> GetByIdAsync(Guid id) => Task.FromResult<FeedingSchedule?>(null);
            public Task AddAsync(FeedingSchedule schedule) => Task.CompletedTask;
            public Task RemoveAsync(FeedingSchedule schedule) => Task.CompletedTask;
            public Task SaveChangesAsync() => Task.CompletedTask;
        }

        private ZooStatisticsService CreateService(
            StubAnimalRepo animalRepo,
            StubEnclosureRepo enclRepo,
            StubScheduleRepo schedRepo)
            => new(animalRepo, enclRepo, schedRepo);

        [Fact]
        public async Task GetStatisticsAsync_ReturnsCorrectCounts()
        {
            // Arrange
            var animals = new List<Animal>
            {
                new Animal("A","a", DateTime.UtcNow.AddYears(-1), Gender.Male,   new Food("X",10)),
                new Animal("B","b", DateTime.UtcNow.AddYears(-2), Gender.Female, new Food("Y",20))
            };
            var enclosures = new List<Enclosure>
            {
                new Enclosure(EnclosureType.Herbivore, 5, capacity:2),
                new Enclosure(EnclosureType.Predator,  10, capacity:3)
            };
            // Заселим первое животное в первый вольер
            enclosures[0].AddAnimal(animals[0]);

            var sched1 = new FeedingSchedule(animals[0].Id, DateTime.UtcNow.AddHours(1), "X");
            var sched2 = new FeedingSchedule(animals[1].Id, DateTime.UtcNow.AddHours(2), "Y");
            sched2.MarkCompleted();

            var animalRepo = new StubAnimalRepo
            {
                GetAllAsyncImpl = () => Task.FromResult((IReadOnlyCollection<Animal>)animals)
            };
            var enclRepo = new StubEnclosureRepo
            {
                GetAllAsyncImpl = () => Task.FromResult((IReadOnlyCollection<Enclosure>)enclosures)
            };
            var schedRepo = new StubScheduleRepo
            {
                GetByAnimalAsyncImpl = _ => Task.FromResult(
                    (IReadOnlyCollection<FeedingSchedule>)new[] { sched1, sched2 })
            };

            var svc = CreateService(animalRepo, enclRepo, schedRepo);

            // Act
            var stats = await svc.GetStatisticsAsync();

            // Assert
            Assert.IsType<ZooStatsDto>(stats);
            Assert.Equal(2, stats.TotalAnimals);
            Assert.Equal(2, stats.TotalEnclosures);
            // свободные места = (2–1) + (3–0) = 4
            Assert.Equal(4, stats.FreeSpaces);
            // незавершённая кормёжка = 1
            Assert.Equal(1, stats.PendingFeedings);
        }
    }
}

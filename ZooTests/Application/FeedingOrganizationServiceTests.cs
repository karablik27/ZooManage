// File: ZooTests/Application/FeedingOrganizationServiceTests.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ZooApplication.Services;
using ZooApplication.Interfaces.Repositories;
using ZooApplication.Interfaces.Services;
using ZooApplication.DTOs;
using ZooDomain.Entities;
using ZooDomain.Enums;
using ZooDomain.Action;
using ZooDomain;

namespace ZooTests.Application
{
    public class FeedingOrganizationServiceTests
    {
        // 1) Stub для IFeedingScheduleRepository
        private class StubScheduleRepo : IFeedingScheduleRepository
        {
            // Позволяет подменять поведение репозитория в тестах:
            public Func<Guid, Task<FeedingSchedule?>> GetByIdAsyncImpl { get; set; }
                = _ => Task.FromResult<FeedingSchedule?>(null);
            public Func<Guid, Task<IReadOnlyCollection<FeedingSchedule>>> GetByAnimalAsyncImpl { get; set; }
                = _ => Task.FromResult((IReadOnlyCollection<FeedingSchedule>)Array.Empty<FeedingSchedule>());

            public bool AddCalled { get; private set; }
            public bool RemoveCalled { get; private set; }
            public bool SaveCalled { get; private set; }

            public Task<FeedingSchedule?> GetByIdAsync(Guid id)
                => GetByIdAsyncImpl(id);

            public Task AddAsync(FeedingSchedule schedule)
            {
                AddCalled = true;
                return Task.CompletedTask;
            }

            public Task RemoveAsync(FeedingSchedule schedule)
            {
                RemoveCalled = true;
                return Task.CompletedTask;
            }

            public Task<IReadOnlyCollection<FeedingSchedule>> GetByAnimalAsync(Guid animalId)
                => GetByAnimalAsyncImpl(animalId);

            public Task SaveChangesAsync()
            {
                SaveCalled = true;
                return Task.CompletedTask;
            }
        }

        // 2) Stub для IAnimalRepository
        private class StubAnimalRepo : IAnimalRepository
        {
            public Func<Guid, Task<Animal?>> GetByIdAsyncImpl { get; set; }
                = _ => Task.FromResult<Animal?>(null);

            public bool SaveCalled { get; private set; }

            public Task<Animal?> GetByIdAsync(Guid id)
                => GetByIdAsyncImpl(id);

            public Task<IReadOnlyCollection<Animal>> GetAllAsync()
                => Task.FromResult((IReadOnlyCollection<Animal>)Array.Empty<Animal>());

            public Task AddAsync(Animal entity)
                => Task.CompletedTask;

            public Task RemoveAsync(Animal entity)
                => Task.CompletedTask;

            public Task SaveChangesAsync()
            {
                SaveCalled = true;
                return Task.CompletedTask;
            }
        }

        // 3) Stub для IEventDispatcher
        private class StubDispatcher : IEventDispatcher
        {
            public List<IDomainEvent> Dispatched { get; } = new();

            public Task DispatchAsync(IEnumerable<IDomainEvent> events)
            {
                Dispatched.AddRange(events);
                return Task.CompletedTask;
            }
        }

        private FeedingOrganizationService CreateService(
            StubScheduleRepo sched,
            StubAnimalRepo animals,
            StubDispatcher disp)
            => new(sched, animals, disp);

        [Fact]
        public async Task GetScheduleAsync_ReturnsMappedDtos()
        {
            var list = new List<FeedingSchedule>
            {
                new FeedingSchedule(Guid.NewGuid(), DateTime.UtcNow.AddHours(1), "Apples"),
                new FeedingSchedule(Guid.NewGuid(), DateTime.UtcNow.AddHours(2), "Bananas")
            };
            var sched = new StubScheduleRepo
            {
                GetByAnimalAsyncImpl = _ => Task.FromResult((IReadOnlyCollection<FeedingSchedule>)list)
            };
            var svc = CreateService(sched, new StubAnimalRepo(), new StubDispatcher());

            var result = await svc.GetScheduleAsync();

            Assert.Equal(2, result.Count);
            Assert.Equal(new[] { "Apples", "Bananas" },
                         result.Select(d => d.FoodType).ToArray());
        }

        [Fact]
        public async Task AddFeedingAsync_UnknownAnimal_ThrowsDomainException()
        {
            var dto = new FeedingScheduleDto(Guid.Empty, Guid.NewGuid(), DateTime.UtcNow, "Food", false);
            var animals = new StubAnimalRepo
            {
                GetByIdAsyncImpl = _ => Task.FromResult<Animal?>(null)
            };
            var svc = CreateService(new StubScheduleRepo(), animals, new StubDispatcher());

            await Assert.ThrowsAsync<DomainException>(
                () => svc.AddFeedingAsync(dto));
        }

        [Fact]
        public async Task AddFeedingAsync_Valid_CreatesDtoAndDispatchesEvent()
        {
            var animal = new Animal("Cat", "Kitty", DateTime.UtcNow.AddYears(-2),
                                    Gender.Female, new Food("Fish", 50));
            var animals = new StubAnimalRepo
            {
                GetByIdAsyncImpl = _ => Task.FromResult<Animal?>(animal)
            };
            var sched = new StubScheduleRepo();
            var disp = new StubDispatcher();
            var svc = CreateService(sched, animals, disp);

            var dtoIn = new FeedingScheduleDto(
                Guid.Empty,
                animal.Id,
                DateTime.UtcNow.AddMinutes(10),
                "Fish",
                false);

            var result = await svc.AddFeedingAsync(dtoIn);

            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(animal.Id, result.AnimalId);
            Assert.Equal("Fish", result.FoodType);
            Assert.False(result.IsCompleted);
            Assert.True(sched.AddCalled, "AddAsync должен быть вызван");
            Assert.True(sched.SaveCalled, "SaveChangesAsync должен быть вызван");
            Assert.Single(disp.Dispatched.OfType<FeedingTimeEvent>());
        }

        [Fact]
        public async Task MarkFedAsync_UnknownSchedule_ThrowsDomainException()
        {
            var sched = new StubScheduleRepo
            {
                GetByIdAsyncImpl = _ => Task.FromResult<FeedingSchedule?>(null)
            };
            var svc = CreateService(sched, new StubAnimalRepo(), new StubDispatcher());

            await Assert.ThrowsAsync<DomainException>(
                () => svc.MarkFedAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task MarkFedAsync_Valid_MarksCompletedAndDispatches()
        {
            var schedule = new FeedingSchedule(Guid.NewGuid(), DateTime.UtcNow.AddMinutes(5), "Hay");
            var sched = new StubScheduleRepo
            {
                GetByIdAsyncImpl = id => Task.FromResult<FeedingSchedule?>(schedule)
            };
            var disp = new StubDispatcher();
            var svc = CreateService(sched, new StubAnimalRepo(), disp);

            await svc.MarkFedAsync(schedule.Id);

            Assert.True(schedule.IsCompleted, "Граф состояния должен быть отмечен");
            Assert.True(sched.SaveCalled, "SaveChangesAsync должен быть вызван");
            Assert.Single(disp.Dispatched.OfType<FeedingTimeEvent>());
        }
    }
}

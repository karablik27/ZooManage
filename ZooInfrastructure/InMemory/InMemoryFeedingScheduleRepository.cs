using System;
using ZooApplication.Interfaces.Repositories;
using ZooDomain.Entities;

namespace ZooInfrastructure.InMemory
{
    /// <inheritdoc />
    public class InMemoryFeedingScheduleRepository : IFeedingScheduleRepository
    {
        private readonly List<FeedingSchedule> _store = new();

        public Task<FeedingSchedule?> GetByIdAsync(Guid id) =>
            Task.FromResult(_store.FirstOrDefault(s => s.Id == id));

        public Task<IReadOnlyCollection<FeedingSchedule>> GetByAnimalAsync(Guid animalId) =>
            Task.FromResult((IReadOnlyCollection<FeedingSchedule>)_store
                .Where(s => s.AnimalId == animalId)
                .ToList());

        public Task AddAsync(FeedingSchedule schedule)
        {
            _store.Add(schedule);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(FeedingSchedule schedule)
        {
            _store.Remove(schedule);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() =>
            Task.CompletedTask;
    }
}


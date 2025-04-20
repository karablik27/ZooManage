using System;
using Zoo.Domain.Entities;
using ZooApplication.Interfaces.Repositories;

namespace ZooInfrastructure.InMemory
{
    /// <inheritdoc />
    public class InMemoryEnclosureRepository : IEnclosureRepository
    {
        private readonly List<Enclosure> _store = new();

        public Task<Enclosure?> GetByIdAsync(Guid id) =>
            Task.FromResult(_store.FirstOrDefault(e => e.Id == id));

        public Task<IReadOnlyCollection<Enclosure>> GetAllAsync() =>
            Task.FromResult((IReadOnlyCollection<Enclosure>)_store.ToList());

        public Task AddAsync(Enclosure enclosure)
        {
            _store.Add(enclosure);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(Enclosure enclosure)
        {
            _store.Remove(enclosure);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() =>
            Task.CompletedTask;
    }
}


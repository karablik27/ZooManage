using System;
using ZooApplication.Interfaces.Repositories;
using ZooDomain.Entities;

namespace ZooInfrastructure.InMemory
{
    /// <inheritdoc />
    public class InMemoryAnimalRepository : IAnimalRepository
    {
        private readonly List<Animal> _store = new();

        public Task<Animal?> GetByIdAsync(Guid id) =>
            Task.FromResult(_store.FirstOrDefault(a => a.Id == id));

        public Task<IReadOnlyCollection<Animal>> GetAllAsync() =>
            Task.FromResult((IReadOnlyCollection<Animal>)_store.ToList());

        public Task AddAsync(Animal animal)
        {
            _store.Add(animal);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(Animal animal)
        {
            _store.Remove(animal);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() =>
            Task.CompletedTask;  // in-memory, so nothing to persist
    }
}


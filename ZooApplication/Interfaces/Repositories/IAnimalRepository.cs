using System;
using ZooDomain.Entities;

namespace ZooApplication.Interfaces.Repositories
{
    /// <summary>
    /// Репозиторий животных.
    /// </summary>
    public interface IAnimalRepository
    {
        Task<Animal?> GetByIdAsync(Guid id);
        Task AddAsync(Animal animal);
        Task RemoveAsync(Animal animal);
        Task<IReadOnlyCollection<Animal>> GetAllAsync();
        Task SaveChangesAsync();
    }
}


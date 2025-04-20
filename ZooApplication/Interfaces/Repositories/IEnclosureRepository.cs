using System;
using Zoo.Domain.Entities;
using ZooDomain.Entities;

namespace ZooApplication.Interfaces.Repositories
{
    /// <summary>
    /// Репозиторий вольеров.
    /// </summary>
    public interface IEnclosureRepository
    {
        Task<Enclosure?> GetByIdAsync(Guid id);
        Task AddAsync(Enclosure enclosure);
        Task RemoveAsync(Enclosure enclosure);
        Task<IReadOnlyCollection<Enclosure>> GetAllAsync();
        Task SaveChangesAsync();
    }
}


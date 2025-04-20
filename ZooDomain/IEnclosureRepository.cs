using System;
using Zoo.Domain.Entities;
using ZooDomain.Entities;

namespace ZooDomain.Interfaces
{
    public interface IEnclosureRepository
    {
        Task<Enclosure> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Enclosure>> ListAllAsync();
        Task AddAsync(Enclosure enclosure);
        Task RemoveAsync(Guid id);
    }
}


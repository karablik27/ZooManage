using System;
using ZooDomain.Entities;

namespace ZooDomain.Interfaces
{
    public interface IFeedingScheduleRepository
    {
        Task<FeedingSchedule> GetByIdAsync(Guid id);
        Task<IReadOnlyList<FeedingSchedule>> ListAllAsync();
        Task AddAsync(FeedingSchedule schedule);
        Task RemoveAsync(Guid id);
    }
}


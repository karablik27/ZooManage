using System;
using ZooApplication.DTOs;

namespace ZooApplication.Interfaces.Services
{
    /// <summary>
    /// Организация кормлений.
    /// </summary>
    public interface IFeedingOrganizationService
    {
        Task<IReadOnlyCollection<FeedingScheduleDto>> GetScheduleAsync();
        Task<FeedingScheduleDto> AddFeedingAsync(FeedingScheduleDto dto);
        Task MarkFedAsync(Guid scheduleId);
    }
}


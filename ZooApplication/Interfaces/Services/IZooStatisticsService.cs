using System;
using ZooApplication.DTOs;

namespace ZooApplication.Interfaces.Services
{
    /// <summary>
    /// Статистика зоопарка.
    /// </summary>
    public interface IZooStatisticsService
    {
        Task<ZooStatsDto> GetStatisticsAsync();
    }
}


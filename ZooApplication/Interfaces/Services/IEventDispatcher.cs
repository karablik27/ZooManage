using System;
using ZooDomain;

namespace ZooApplication.Interfaces.Services
{
    /// <summary>
    /// Публикация доменных событий.
    /// </summary>
    public interface IEventDispatcher
    {
        Task DispatchAsync(IEnumerable<IDomainEvent> events);
    }
}


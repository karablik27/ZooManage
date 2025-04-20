using System;
using System.Text.Json;
using ZooApplication.Interfaces.Services;

using ZooDomain;

namespace ZooInfrastructure
{
    /// <inheritdoc />
    public class EventDispatcher : IEventDispatcher
    {
        /// <summary>
        /// Публикует доменные события, выводя их в консоль.
        /// </summary>
        public Task DispatchAsync(IEnumerable<IDomainEvent> events)
        {
            foreach (var evt in events)
            {
                // Выводим имя события и его данные в формате JSON
                Console.WriteLine(
                    "[DomainEvent] {0}: {1}",
                    evt.GetType().Name,
                    JsonSerializer.Serialize(evt)
                );
            }
            return Task.CompletedTask;
        }
    }
}


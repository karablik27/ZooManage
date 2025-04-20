using System;
namespace ZooDomain.Action
{
    /// <summary>
    /// Событие перемещения животного между вольерами.
    /// </summary>
    public sealed record AnimalMovedEvent(
        Guid AnimalId,
        Guid FromEnclosureId,
        Guid ToEnclosureId,
        DateTime OccurredAt
    ) : IDomainEvent
    {
        /// <summary>
        /// Перегрузка, чтобы не передавать вручную OccurredAt.
        /// </summary>
        public AnimalMovedEvent(
            Guid animalId,
            Guid fromEnclosureId,
            Guid toEnclosureId
        ) : this(animalId, fromEnclosureId, toEnclosureId, DateTime.UtcNow)
        { }
    }
}


using System;
namespace ZooDomain
{
    /// <summary>
    /// Базовый класс сущностей с Id и коллекцией доменных событий.
    /// </summary>
    public abstract class Entity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();

        private readonly List<IDomainEvent> _domainEvents = new();

        /// <summary>
        /// Доменные события, произошедшие с этой сущностью.
        /// </summary>
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Добавить доменное событие.
        /// </summary>
        public void AddDomainEvent(IDomainEvent domainEvent) =>
            _domainEvents.Add(domainEvent);

        /// <summary>
        /// Очистить после публикации.
        /// </summary>
        public void ClearDomainEvents() =>
            _domainEvents.Clear();
    }
}


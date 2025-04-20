using System;
namespace ZooDomain.Action
{
    public sealed record EnclosureCleanedEvent(Guid EnclosureId, DateTime CleanedAt) : IDomainEvent;
}
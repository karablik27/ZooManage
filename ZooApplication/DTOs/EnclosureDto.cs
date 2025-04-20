using System;
namespace ZooApplication.DTOs
{
    public record EnclosureDto(
        Guid Id,
        string Type,
        double SizeInSquareMeters,
        int CurrentCount,
        int Capacity,
        bool IsDirty,
        DateTime? LastCleanedAt
    );
}
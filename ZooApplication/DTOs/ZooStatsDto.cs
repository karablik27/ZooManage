using System;
namespace ZooApplication.DTOs
{
    public record ZooStatsDto(
        int TotalAnimals,
        int TotalEnclosures,
        int FreeSpaces,
        int PendingFeedings
    );
}


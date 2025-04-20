using System;
namespace ZooApplication.DTOs
{
    public record AnimalDto(
       Guid Id,
       string Species,
       string Name,
       DateTime DateOfBirth,
       string Gender,
       string FavoriteFood,
       string HealthStatus,
       Guid? EnclosureId
   );
}


using System;
using System.Linq;
using System.Threading.Tasks;
using ZooApplication.DTOs;
using ZooApplication.Interfaces.Repositories;
using ZooApplication.Interfaces.Services;

namespace ZooApplication.Services
{
    /// <inheritdoc />
    public class ZooStatisticsService : IZooStatisticsService
    {
        private readonly IAnimalRepository _animalRepo;
        private readonly IEnclosureRepository _enclosureRepo;
        private readonly IFeedingScheduleRepository _schedRepo;

        public ZooStatisticsService(
            IAnimalRepository animalRepo,
            IEnclosureRepository enclosureRepo,
            IFeedingScheduleRepository schedRepo
        )
        {
            _animalRepo = animalRepo;
            _enclosureRepo = enclosureRepo;
            _schedRepo = schedRepo;
        }

        public async Task<ZooStatsDto> GetStatisticsAsync()
        {
            var animals = await _animalRepo.GetAllAsync();
            var enclosures = await _enclosureRepo.GetAllAsync();
            var schedules = await _schedRepo.GetByAnimalAsync(Guid.Empty);

            var totalAnimals = animals.Count;
            var totalEnclosures = enclosures.Count;
            var freeSpaces = enclosures.Sum(e => e.Capacity - e.AnimalIds.Count);
            var pendingFeedings = schedules.Count(s => !s.IsCompleted);

            return new ZooStatsDto(
                totalAnimals,
                totalEnclosures,
                freeSpaces,
                pendingFeedings
            );
        }
    }
}

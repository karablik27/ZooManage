using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZooApplication.DTOs;
using ZooApplication.Interfaces.Repositories;
using ZooApplication.Interfaces.Services;
using ZooDomain;
using ZooDomain.Entities;

namespace ZooApplication.Services
{
    /// <inheritdoc />
    public class FeedingOrganizationService : IFeedingOrganizationService
    {
        private readonly IFeedingScheduleRepository _schedRepo;
        private readonly IAnimalRepository _animalRepo;
        private readonly IEventDispatcher _dispatcher;

        public FeedingOrganizationService(
            IFeedingScheduleRepository schedRepo,
            IAnimalRepository animalRepo,
            IEventDispatcher dispatcher
        )
        {
            _schedRepo = schedRepo;
            _animalRepo = animalRepo;
            _dispatcher = dispatcher;
        }

        public async Task<IReadOnlyCollection<FeedingScheduleDto>> GetScheduleAsync()
        {
            var list = await _schedRepo.GetByAnimalAsync(Guid.Empty);
            return list
                .Select(s => new FeedingScheduleDto(
                    s.Id,
                    s.AnimalId,
                    s.FeedingTime,
                    s.FoodType,
                    s.IsCompleted
                ))
                .ToList();
        }

        // Исправлено: теперь возвращаем Task<FeedingScheduleDto>
        public async Task<FeedingScheduleDto> AddFeedingAsync(FeedingScheduleDto dto)
        {
            // Проверяем существование животного
            var animal = await _animalRepo.GetByIdAsync(dto.AnimalId)
                         ?? throw new DomainException("Животное не найдено.");

            // Создаём и сохраняем новую запись
            var schedule = new FeedingSchedule(
                dto.AnimalId,
                dto.FeedingTime,
                dto.FoodType
            );

            await _schedRepo.AddAsync(schedule);
            await _schedRepo.SaveChangesAsync();

            // Публикация событий
            await _dispatcher.DispatchAsync(schedule.DomainEvents);
            schedule.ClearDomainEvents();

            // Возвращаем DTO с заполненным Id и текущим флагом IsCompleted
            return new FeedingScheduleDto(
                schedule.Id,
                schedule.AnimalId,
                schedule.FeedingTime,
                schedule.FoodType,
                schedule.IsCompleted
            );
        }

        public async Task MarkFedAsync(Guid scheduleId)
        {
            var schedule = await _schedRepo.GetByIdAsync(scheduleId)
                           ?? throw new DomainException("Запись о кормлении не найдена.");

            schedule.MarkCompleted(); // поднимет FeedingTimeEvent

            await _schedRepo.SaveChangesAsync();
            await _dispatcher.DispatchAsync(schedule.DomainEvents);
            schedule.ClearDomainEvents();
        }
    }
}

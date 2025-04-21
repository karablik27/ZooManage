// File: ZooApplication/Services/FeedingOrganizationService.cs
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ZooApplication.DTOs;
using ZooApplication.Interfaces.Repositories;
using ZooApplication.Interfaces.Services;
using ZooDomain;
using ZooDomain.Action;
using ZooDomain.Entities;

namespace ZooApplication.Services
{
    public class FeedingOrganizationService : IFeedingOrganizationService
    {
        private readonly IFeedingScheduleRepository _schedRepo;
        private readonly IAnimalRepository _animalRepo;
        private readonly IEventDispatcher _dispatcher;

        public FeedingOrganizationService(
            IFeedingScheduleRepository schedRepo,
            IAnimalRepository animalRepo,
            IEventDispatcher dispatcher)
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
                .ToList()
                .AsReadOnly();
        }

        public async Task<FeedingScheduleDto> AddFeedingAsync(FeedingScheduleDto dto)
        {
            var animal = await _animalRepo.GetByIdAsync(dto.AnimalId)
                         ?? throw new DomainException("Животное не найдено.");

            var schedule = new FeedingSchedule(
                dto.AnimalId,
                dto.FeedingTime,
                dto.FoodType
            );

            await _schedRepo.AddAsync(schedule);
            await _schedRepo.SaveChangesAsync();

            // Поднимаем событие сразу после создания
            schedule.AddDomainEvent(new FeedingTimeEvent(
                schedule.AnimalId,
                schedule.FeedingTime,
                schedule.FoodType
            ));
            await _dispatcher.DispatchAsync(schedule.DomainEvents);
            schedule.ClearDomainEvents();

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

            schedule.MarkCompleted();  // установит IsCompleted и добавит FeedingTimeEvent

            await _schedRepo.SaveChangesAsync();
            await _dispatcher.DispatchAsync(schedule.DomainEvents);
            schedule.ClearDomainEvents();
        }
    }
}

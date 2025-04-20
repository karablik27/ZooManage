// File: ZooApplication/Services/AnimalTransferService.cs
using System;
using System.Threading.Tasks;
using Zoo.Domain.Entities;
using ZooApplication.Interfaces.Repositories;
using ZooApplication.Interfaces.Services;
using ZooDomain;
using ZooDomain.Entities;

namespace ZooApplication.Services
{
    public class AnimalTransferService : IAnimalTransferService
    {
        private readonly IAnimalRepository _animalRepo;
        private readonly IEnclosureRepository _enclosureRepo;
        private readonly IEventDispatcher _dispatcher;

        public AnimalTransferService(
            IAnimalRepository animalRepo,
            IEnclosureRepository enclosureRepo,
            IEventDispatcher dispatcher
        )
        {
            _animalRepo = animalRepo;
            _enclosureRepo = enclosureRepo;
            _dispatcher = dispatcher;
        }

        public async Task MoveAsync(Guid animalId, Guid targetEnclosureId)
        {
            // 1) Загрузить животное
            var animal = await _animalRepo.GetByIdAsync(animalId)
                         ?? throw new DomainException("Животное не найдено.");

            // 2) Загрузить целевой вольер
            var target = await _enclosureRepo.GetByIdAsync(targetEnclosureId)
                         ?? throw new DomainException("Целевой вольер не найден.");

            // 3) Определить текущий вольер (если есть)
            Enclosure? current = null;
            if (animal.EnclosureId.HasValue)
            {
                current = await _enclosureRepo.GetByIdAsync(animal.EnclosureId.Value)
                          ?? throw new DomainException("Текущий вольер не найден.");
            }

            // 4) Проверить вместимость нового вольера
            if (target.AnimalIds.Count >= target.Capacity)
                throw new DomainException("Целевой вольер переполнен.");

            // 5) Убрать из старого и добавить в новый
            current?.RemoveAnimal(animal);
            target.AddAnimal(animal);

            // 6) Обновить у животного ссылку на вольер (поднимет AnimalMovedEvent)
            animal.MoveTo(target.Id);

            // 7) Сохранить изменения
            await _enclosureRepo.SaveChangesAsync();
            await _animalRepo.SaveChangesAsync();

            // 8) Опубликовать доменные события
            await _dispatcher.DispatchAsync(animal.DomainEvents);
            animal.ClearDomainEvents();
        }
    }
}

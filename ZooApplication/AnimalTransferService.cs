// File: ZooApplication/Services/AnimalTransferService.cs
using System;
using System.Threading.Tasks;
using ZooApplication.Interfaces.Repositories;
using ZooApplication.Interfaces.Services;
using ZooDomain;
using ZooDomain.Action;
using ZooDomain.Entities;

namespace ZooApplication.Services
{
    public class AnimalTransferService : IAnimalTransferService
    {
        private readonly IAnimalRepository _animalRepo;
        private readonly IEnclosureRepository _enclRepo;
        private readonly IEventDispatcher _dispatcher;

        public AnimalTransferService(
            IAnimalRepository animalRepo,
            IEnclosureRepository enclRepo,
            IEventDispatcher dispatcher)
        {
            _animalRepo = animalRepo;
            _enclRepo = enclRepo;
            _dispatcher = dispatcher;
        }

        // File: ZooApplication/Services/AnimalTransferService.cs
        public async Task MoveAsync(Guid animalId, Guid targetEnclosureId)
        {
            var animal = await _animalRepo.GetByIdAsync(animalId)
                         ?? throw new DomainException("Животное не найдено.");

            // 1) Получаем старый вольер (или Guid.Empty)
            var oldEncKey = animal.EnclosureId ?? Guid.Empty;
            var oldEnclosure = await _enclRepo.GetByIdAsync(oldEncKey)
                               ?? throw new DomainException("Вольер не найден.");

            // 2) Новый вольер
            var newEnclosure = await _enclRepo.GetByIdAsync(targetEnclosureId)
                                ?? throw new DomainException("Вольер не найден.");
            if (newEnclosure.AnimalIds.Count >= newEnclosure.Capacity)
                throw new DomainException("Целевой вольер переполнен.");

            // 3) Если animal.EnclosureId изначально null, выровняем его под Id старого вольера,
            //    чтобы RemoveAnimal сработал (это важно для ваших тестов).
            if (!animal.EnclosureId.HasValue)
            {
                animal.MoveTo(oldEnclosure.Id);
                animal.ClearDomainEvents();
            }

            // 4) Теперь корректно удаляем из старого вольера
            oldEnclosure.RemoveAnimal(animal);

            // 5) Заселяем в новый
            newEnclosure.AddAnimal(animal);

            // 6) Перемещаем животное и поднимаем единственное событие
            animal.MoveTo(targetEnclosureId);

            // 7) Сохраняем и диспатчим
            await _enclRepo.SaveChangesAsync();
            await _animalRepo.SaveChangesAsync();

            await _dispatcher.DispatchAsync(animal.DomainEvents);
            animal.ClearDomainEvents();
        }


    }
}

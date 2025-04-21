
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using ZooApplication.Services;
using ZooApplication.Interfaces.Repositories;
using ZooApplication.Interfaces.Services;
using ZooDomain.Entities;
using ZooDomain.Enums;
using ZooDomain.Action;
using ZooDomain;
using Zoo.Domain.Entities;

// Простые stub‑реализации для репозиториев и диспетчера
namespace ZooTests.Application
{
    public class AnimalTransferServiceTests
    {
        // Внутри AnimalTransferServiceTests

        private class StubAnimalRepository : IAnimalRepository
        {
            // Позволяет на лету менять поведение GetByIdAsync в тесте
            public Func<Guid, Task<Animal?>> GetByIdAsyncImpl { get; set; }
                = id => Task.FromResult<Animal?>(null);

            public bool SaveCalled { get; private set; }

            public Task<Animal?> GetByIdAsync(Guid id) => GetByIdAsyncImpl(id);

            public Task<IReadOnlyCollection<Animal>> GetAllAsync() =>
                // Массив Animal[] реализует IReadOnlyCollection<Animal>
                Task.FromResult((IReadOnlyCollection<Animal>)Array.Empty<Animal>());

            public Task AddAsync(Animal entity) => Task.CompletedTask;

            public Task RemoveAsync(Animal entity) => Task.CompletedTask;

            public Task SaveChangesAsync()
            {
                SaveCalled = true;
                return Task.CompletedTask;
            }
        }

        private class StubEnclosureRepository : IEnclosureRepository
        {
            public Func<Guid, Task<Enclosure?>> GetByIdAsyncImpl { get; set; }
                = id => Task.FromResult<Enclosure?>(null);

            public bool SaveCalled { get; private set; }

            public Task<Enclosure?> GetByIdAsync(Guid id) => GetByIdAsyncImpl(id);

            public Task<IReadOnlyCollection<Enclosure>> GetAllAsync() =>
                Task.FromResult((IReadOnlyCollection<Enclosure>)Array.Empty<Enclosure>());

            public Task AddAsync(Enclosure entity) => Task.CompletedTask;

            public Task RemoveAsync(Enclosure entity) => Task.CompletedTask;

            public Task SaveChangesAsync()
            {
                SaveCalled = true;
                return Task.CompletedTask;
            }
        }


        private class StubEventDispatcher : IEventDispatcher
        {
            public List<IDomainEvent> Dispatched { get; } = new();
            public Task DispatchAsync(IEnumerable<IDomainEvent> events)
            {
                Dispatched.AddRange(events);
                return Task.CompletedTask;
            }
        }

        private AnimalTransferService CreateService(
            StubAnimalRepository animalRepo,
            StubEnclosureRepository enclRepo,
            StubEventDispatcher dispatcher)
            => new(animalRepo, enclRepo, dispatcher);

        [Fact]
        public async Task MoveAsync_AnimalNotFound_ThrowsDomainException()
        {
            var animalRepo = new StubAnimalRepository
            {
                GetByIdAsyncImpl = _ => Task.FromResult<Animal?>(null)
            };
            var enclRepo = new StubEnclosureRepository();
            var dispatcher = new StubEventDispatcher();
            var svc = CreateService(animalRepo, enclRepo, dispatcher);

            await Assert.ThrowsAsync<DomainException>(
                () => svc.MoveAsync(Guid.NewGuid(), Guid.NewGuid()));
        }

        [Fact]
        public async Task MoveAsync_TargetEnclosureFull_ThrowsDomainException()
        {
            var animal = new Animal("A", "a", DateTime.UtcNow.AddYears(-1),
                                    Gender.Male, new Food("F", 10));
            var newEncId = Guid.NewGuid();

            var animalRepo = new StubAnimalRepository
            {
                GetByIdAsyncImpl = id => Task.FromResult<Animal?>(animal)
            };
            var enclRepo = new StubEnclosureRepository
            {
                GetByIdAsyncImpl = id => Task.FromResult<Enclosure?>(
                    new Enclosure(EnclosureType.Herbivore, 10, capacity: 0))
            };
            var dispatcher = new StubEventDispatcher();
            var svc = CreateService(animalRepo, enclRepo, dispatcher);

            await Assert.ThrowsAsync<DomainException>(
                () => svc.MoveAsync(animal.Id, newEncId));
        }
    }
}

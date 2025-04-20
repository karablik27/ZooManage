using System;
using System.Collections.Generic;
using ZooDomain;
using ZooDomain.Action;
using ZooDomain.Entities;
using ZooDomain.Enums;

namespace Zoo.Domain.Entities
{
    /// <summary>
    /// Агрегат «Вольер».
    /// Хранит список животных по их Id, а не просто счётчик.
    /// </summary>
    public class Enclosure : Entity
    {
        public EnclosureType Type { get; private set; }
        public double Area { get; private set; }
        public int Capacity { get; private set; }

        /// <summary>
        /// Список Id животных, заселённых в этом вольере.
        /// </summary>
        private readonly List<Guid> _animalIds = new();
        public IReadOnlyCollection<Guid> AnimalIds => _animalIds.AsReadOnly();

        /// <summary>Когда вольер последний раз убирали</summary>
        public DateTime? LastCleanedAt { get; private set; }

        /// <summary>Флаг: требуется уборка</summary>
        public bool IsDirty { get; private set; }

        private Enclosure() { } // для ORM/десериализации

        public Enclosure(EnclosureType type, double area, int capacity)
        {
            if (area <= 0)
                throw new DomainException("Площадь вольера должна быть положительной.");
            if (capacity <= 0)
                throw new DomainException("Вместимость должна быть положительной.");

            Type = type;
            Area = area;
            Capacity = capacity;
            IsDirty = false;
            LastCleanedAt = null;
        }

        /// <summary>
        /// Добавить животное в вольер.
        /// </summary>
        public void AddAnimal(Animal animal)
        {
            if (_animalIds.Count >= Capacity)
                throw new DomainException("Вольер переполнен.");

            _animalIds.Add(animal.Id);
            IsDirty = true;
        }

        /// <summary>
        /// Убрать животное из вольера.
        /// </summary>
        public void RemoveAnimal(Animal animal)
        {
            if (!_animalIds.Remove(animal.Id))
                throw new DomainException("Такое животное в вольере не найдено.");

            IsDirty = true;
        }

        /// <summary>
        /// Провести уборку: сбросить флаг и зафиксировать время, поднять событие.
        /// </summary>
        public void Clean()
        {
            if (!IsDirty)
                throw new DomainException("Уборка не требуется.");

            LastCleanedAt = DateTime.UtcNow;
            IsDirty = false;

            AddDomainEvent(new EnclosureCleanedEvent(
                EnclosureId: Id,
                CleanedAt: LastCleanedAt.Value
            ));
        }
    }
}

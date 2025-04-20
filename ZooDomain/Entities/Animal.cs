using System;
using ZooDomain.Action;
using ZooDomain.Enums;

namespace ZooDomain.Entities
{
    /// <summary>
    /// Агрегат «Животное»
    /// </summary>
    public class Animal : Entity
    {
        public string Species { get; private set; }
        public string Name { get; private set; }
        public DateTime DateOfBirth { get; private set; }
        public Gender Gender { get; private set; }
        public Food FavoriteFood { get; private set; }
        public HealthStatus Status { get; private set; }
        public Guid? EnclosureId { get; private set; }

        private Animal() { }

        public Animal(
            string species,
            string name,
            DateTime dateOfBirth,
            Gender gender,
            Food favoriteFood
        )
        {
            if (string.IsNullOrWhiteSpace(species))
                throw new DomainException("Вид не может быть пустым.");
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Имя не может быть пустым.");
            Species = species;
            Name = name;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            FavoriteFood = favoriteFood ?? throw new DomainException("Укажите любимую еду.");
            Status = HealthStatus.Healthy;
        }

        public void Feed(Food food)
        {
            if (Status == HealthStatus.Sick)
                throw new DomainException("Нельзя кормить больное животное.");
            if (food.Name != FavoriteFood.Name)
                throw new DomainException(
                    $"Это не любимая еда («{FavoriteFood.Name}»)."
                );
        }

        public void Heal() => Status = HealthStatus.Healthy;
        public void MarkSick() => Status = HealthStatus.Sick;

        public void MoveTo(Guid newEnclosureId)
        {
            if (EnclosureId == newEnclosureId)
                throw new DomainException("Животное уже в этом вольере.");

            var oldId = EnclosureId ?? Guid.Empty;
            EnclosureId = newEnclosureId;
            AddDomainEvent(new AnimalMovedEvent(
                Id,
                oldId,
                newEnclosureId
            ));
        }
    }
}


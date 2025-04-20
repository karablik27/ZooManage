using System;
using ZooDomain.Action;

namespace ZooDomain.Entities
{
    /// <summary>
    /// Агрегат «Расписание кормления»
    /// </summary>
    public class FeedingSchedule : Entity
    {
        public Guid AnimalId { get; private set; }
        public DateTime FeedingTime { get; private set; }
        public string FoodType { get; private set; }
        public bool IsCompleted { get; private set; }

        private FeedingSchedule() { }

        public FeedingSchedule(
            Guid animalId,
            DateTime feedingTime,
            string foodType
        )
        {
            if (animalId == Guid.Empty)
                throw new DomainException("Идентификатор животного обязателен.");
            if (feedingTime < DateTime.UtcNow)
                throw new DomainException("Время кормления должно быть в будущем.");
            if (string.IsNullOrWhiteSpace(foodType))
                throw new DomainException("Тип пищи не может быть пустым.");

            AnimalId = animalId;
            FeedingTime = feedingTime;
            FoodType = foodType;
            IsCompleted = false;
        }

        /// <summary>
        /// Изменить время кормления.
        /// </summary>
        public void Reschedule(DateTime newTime)
        {
            if (newTime < DateTime.UtcNow)
                throw new DomainException("Новое время должно быть в будущем.");
            FeedingTime = newTime;
            IsCompleted = false;
        }

        /// <summary>
        /// Отметить выполнение кормления и поднять событие.
        /// </summary>
        public void MarkCompleted()
        {
            if (IsCompleted)
                throw new DomainException("Кормление уже отмечено.");
            IsCompleted = true;

            AddDomainEvent(new FeedingTimeEvent(
                AnimalId,
                FeedingTime,
                FoodType
            ));
        }
    }
}


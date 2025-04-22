# ZooManage — система управления зоопарком

## 📋 Отчёт по проекту

### ✅ A. Реализованный функционал

| №  | Функциональность                                      | Где реализовано (классы / модули)                           |
|----|--------------------------------------------------------|-------------------------------------------------------------|
| 1  | Добавление и управление животными                      | `ZooDomain.Entities.Animal`                                |
| 2  | Управление расписанием кормлений                      | `ZooDomain.Entities.FeedingSchedule`                       |
| 3  | Логика кормления и бизнес-операции                     | `ZooApplication.Services.FeedingOrganizationService`       |
| 4  | Репозитории для хранения и получения данных            | Интерфейсы: `IAnimalRepository`, `IFeedingRepository` в `ZooDomain`<br>Реализация: `ZooInfrastructure.Repositories.*` |
| 5  | Интеграция между слоями и сценарии использования       | `ZooApplication.UseCases.*`                                |
| 6  | Модульное тестирование логики                          | `ZooTests.Application.*`                                   |

---

### 🧠 B. Использованные концепции Domain-Driven Design

| Концепция DDD             | Реализация (где применено)                                         |
|--------------------------|--------------------------------------------------------------------|
| **Entity**               | `Animal`, `FeedingSchedule` — в `ZooDomain.Entities`               |
| **Aggregate Root**       | `Animal` — корневая сущность управления                            |
| **Repository**           | Интерфейсы: `IAnimalRepository`, `IFeedingRepository`<br>Реализация: `ZooInfrastructure.Repositories.*` |
| **Service (Domain Service)** | `FeedingOrganizationService` — бизнес-логика кормления              |
| **Bounded Context**      | Разделение по проектам: `ZooDomain`, `ZooApplication`, `ZooInfrastructure`, `ZooTests` |
| **Use Cases**            | Сценарии в `ZooApplication` — например, обработка кормления        |

---

### 🏗 Принципы Clean Architecture

| Принцип                          | Как реализовано                                                |
|----------------------------------|----------------------------------------------------------------|
| **Разделение слоёв**            | `Domain`, `Application`, `Infrastructure`, `Tests`            |
| **Направление зависимостей внутрь** | Инфраструктура зависит от ядра, не наоборот                   |
| **Интерфейсы на границах**      | Использование `IRepository`, `IService`, внедрение зависимостей через DI |
| **Изоляция бизнес-логики**      | `ZooDomain` и `ZooApplication` не зависят от хранения данных или UI |
| **Тестируемость**               | Вся логика покрывается unit-тестами в `ZooTests`              |

---

### 🔧 Используемые технологии

- .NET 7
- xUnit
- Moq
- FluentAssertions
- Clean Architecture
- Domain-Driven Design (DDD)


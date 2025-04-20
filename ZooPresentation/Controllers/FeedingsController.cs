using System;
using Microsoft.AspNetCore.Mvc;
using ZooApplication.DTOs;
using ZooApplication.Interfaces.Repositories;
using ZooApplication.Interfaces.Services;
using ZooPresentation.Models;

namespace ZooPresentation.Controllers
{
    [ApiController]
    [Route("api/feedings")]
    public class FeedingsController : ControllerBase
    {
        private readonly IFeedingScheduleRepository _schedRepo;
        private readonly IFeedingOrganizationService _feedingSvc;

        public FeedingsController(
            IFeedingScheduleRepository schedRepo,
            IFeedingOrganizationService feedingSvc)
        {
            _schedRepo = schedRepo;
            _feedingSvc = feedingSvc;
        }

        [HttpGet]
        public async Task<IEnumerable<FeedingScheduleDto>> GetAll()
        {
            // Для демонстрации: получаем по всем животным Guid.Empty
            var list = await _schedRepo.GetByAnimalAsync(Guid.Empty);
            return list.Select(s => new FeedingScheduleDto(
                s.Id, s.AnimalId, s.FeedingTime, s.FoodType, s.IsCompleted
            ));
        }

        [HttpPost]
        public async Task<ActionResult<FeedingScheduleDto>> Create([FromBody] CreateFeedingRequest r)
        {
            var dto = await _feedingSvc.AddFeedingAsync(new FeedingScheduleDto(
                Guid.Empty, // Id генерируется внутри
                r.AnimalId,
                r.FeedingTime,
                r.FoodType,
                false
            ));
            return CreatedAtAction(nameof(GetAll), new { id = dto.Id }, dto);
        }

        [HttpPost("{id:guid}/execute")]
        public async Task<IActionResult> Execute(Guid id)
        {
            await _feedingSvc.MarkFedAsync(id);
            return NoContent();
        }
    }
}


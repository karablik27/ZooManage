using System;
using Microsoft.AspNetCore.Mvc;
using ZooApplication.DTOs;
using ZooApplication.Interfaces.Repositories;
using ZooApplication.Interfaces.Services;
using ZooDomain;
using ZooDomain.Entities;
using ZooPresentation.Models;

namespace ZooPresentation.Controllers
{
    [ApiController]
    [Route("api/animals")]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalRepository _repo;
        private readonly IAnimalTransferService _transfer;

        public AnimalsController(
            IAnimalRepository repo,
            IAnimalTransferService transfer)
        {
            _repo = repo;
            _transfer = transfer;
        }

        [HttpGet]
        public async Task<IEnumerable<AnimalDto>> GetAll()
        {
            var all = await _repo.GetAllAsync();
            return all.Select(a => new AnimalDto(
                a.Id,
                a.Species,
                a.Name,
                a.DateOfBirth,
                a.Gender.ToString(),
                a.FavoriteFood.Name,
                a.Status.ToString(),
                a.EnclosureId
            ));
        }

        [HttpPost]
        public async Task<ActionResult<AnimalDto>> Create([FromBody] CreateAnimalRequest r)
        {
            var animal = new Animal(
                r.Species,
                r.Name,
                r.DateOfBirth,
                r.Gender,
                new Food(r.FavoriteFoodName, r.Calories)
            );

            await _repo.AddAsync(animal);
            await _repo.SaveChangesAsync();

            var dto = new AnimalDto(
                animal.Id,
                animal.Species,
                animal.Name,
                animal.DateOfBirth,
                animal.Gender.ToString(),
                animal.FavoriteFood.Name,
                animal.Status.ToString(),
                animal.EnclosureId
            );
            return CreatedAtAction(nameof(GetAll), new { id = animal.Id }, dto);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var a = await _repo.GetByIdAsync(id);
            if (a is null) return NotFound();
            await _repo.RemoveAsync(a);
            await _repo.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id:guid}/move/{enclosureId:guid}")]
        public async Task<IActionResult> Move(Guid id, Guid enclosureId)
        {
            await _transfer.MoveAsync(id, enclosureId);
            return NoContent();
        }
    }
}


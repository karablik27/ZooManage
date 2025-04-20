using System;
using Microsoft.AspNetCore.Mvc;
using Zoo.Domain.Entities;
using ZooApplication.DTOs;
using ZooApplication.Interfaces.Repositories;
using ZooPresentation.Models;

namespace ZooPresentation.Controllers
{
    [ApiController]
    [Route("api/enclosures")]
    public class EnclosuresController : ControllerBase
    {
        private readonly IEnclosureRepository _repo;

        public EnclosuresController(IEnclosureRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<IEnumerable<EnclosureDto>> GetAll()
        {
            var all = await _repo.GetAllAsync();
            return all.Select(e => new EnclosureDto(
                e.Id,
                e.Type.ToString(),
                e.Area,
                e.AnimalIds.Count,
                e.Capacity,
                e.IsDirty,
                e.LastCleanedAt
            ));
        }

        [HttpPost]
        public async Task<ActionResult<EnclosureDto>> Create([FromBody] CreateEnclosureRequest r)
        {
            var enc = new Enclosure(r.Type, r.Area, r.Capacity);
            await _repo.AddAsync(enc);
            await _repo.SaveChangesAsync();

            var dto = new EnclosureDto(
                enc.Id,
                enc.Type.ToString(),
                enc.Area,
                enc.AnimalIds.Count,
                enc.Capacity,
                enc.IsDirty,
                enc.LastCleanedAt
            );
            return CreatedAtAction(nameof(GetAll), new { id = enc.Id }, dto);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e is null) return NotFound();
            await _repo.RemoveAsync(e);
            await _repo.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id:guid}/clean")]
        public async Task<IActionResult> Clean(Guid id)
        {
            var e = await _repo.GetByIdAsync(id);
            if (e is null) return NotFound();
            e.Clean();
            await _repo.SaveChangesAsync();
            return NoContent();
        }
    }
}


using System;
using Microsoft.AspNetCore.Mvc;
using ZooApplication.DTOs;
using ZooApplication.Interfaces.Services;

namespace ZooPresentation.Controllers
{
    [ApiController]
    [Route("api/zoo")]
    public class StatisticsController : ControllerBase
    {
        private readonly IZooStatisticsService _stats;

        public StatisticsController(IZooStatisticsService stats) => _stats = stats;

        [HttpGet("stats")]
        public async Task<ZooStatsDto> Get() => await _stats.GetStatisticsAsync();
    }
}


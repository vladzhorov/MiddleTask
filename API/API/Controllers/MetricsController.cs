using API.Models;
using API.Repositories;
using Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private readonly IMetricReadRepository _repository;

    public MetricsController(IMetricReadRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<MetricDocument>>> GetMetrics(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 100)
    {
        if (skip < 0)
        {
            return BadRequest("Query parameter 'skip' must be >= 0.");
        }

        if (take <= 0 || take > 500)
        {
            return BadRequest("Query parameter 'take' must be in range 1..500.");
        }

        var items = await _repository.GetMetricsPageAsync(skip, take);

        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MetricDocument>> GetMetricById(string id)
    {
        var metric = await _repository.GetByIdAsync(id);
        if (metric is null)
        {
            return NotFound();
        }

        return Ok(metric);
    }

    [HttpGet("aggregations/by-type")]
    public async Task<ActionResult<IReadOnlyList<MetricTypeAggregation>>> GetAggregationsByType()
    {
        var result = await _repository.GetAggregationsByTypeAsync();
        return Ok(result);
    }
}


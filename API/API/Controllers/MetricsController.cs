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
    public ActionResult<IQueryable<MetricDocument>> GetMetrics()
    {
        return Ok(_repository.GetMetrics());
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


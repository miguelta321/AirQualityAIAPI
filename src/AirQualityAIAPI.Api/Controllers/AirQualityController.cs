using AirQualityAIAPI.Application.DTOs;
using AirQualityAIAPI.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AirQualityAIAPI.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AirQualityController : ControllerBase
{
    private readonly IAirQualityService _service;

    public AirQualityController(IAirQualityService service)
    {
        _service = service;
    }

    /// <summary>Returns all air quality readings.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AirQualityReadingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var readings = await _service.GetAllReadingsAsync();
        return Ok(readings);
    }

    /// <summary>Returns a single air quality reading by its ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AirQualityReadingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var reading = await _service.GetReadingByIdAsync(id);
        return reading is null ? NotFound() : Ok(reading);
    }

    /// <summary>Returns air quality readings filtered by location.</summary>
    [HttpGet("location/{location}")]
    [ProducesResponseType(typeof(IEnumerable<AirQualityReadingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByLocation(string location)
    {
        var readings = await _service.GetReadingsByLocationAsync(location);
        return Ok(readings);
    }

    /// <summary>Creates a new air quality reading.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AirQualityReadingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAirQualityReadingRequest request)
    {
        var created = await _service.CreateReadingAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>Updates an existing air quality reading.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AirQualityReadingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAirQualityReadingRequest request)
    {
        var updated = await _service.UpdateReadingAsync(id, request);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>Deletes an air quality reading.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteReadingAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicStore.Dto.Common;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Services;

namespace MusicStore.Api.Controllers;

[ApiController]
[Route("api/concerts")]
public class ConcertsController(IConcertService _service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PaginatedResult<ConcertResponseDto>>> Get(
        [FromQuery] string? title,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _service.GetAsync(title, page, pageSize);
        return Ok(result);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ConcertResponseDto>> Get(int id)
    {
        var result = await _service.GetAsync(id);
        return Ok(result);
    }
    
    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<ConcertResponseDto>> Post(
        [FromForm] ConcertRequestDto request)
    {
        var created = await _service.AddAsync(request);

        return CreatedAtAction(
            nameof(Get),
            new { id = created.Id },
            created);
    }
    
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<ActionResult<ConcertResponseDto>> Put(
        int id,
        [FromForm] ConcertRequestDto request)
    {
        var updated = await _service.UpdateAsync(id, request);
        return Ok(updated);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
    
    [HttpPatch("{id:int}/finalize")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Finalize(int id)
    {
        await _service.FinalizeAsync(id);
        return NoContent();
    }
}

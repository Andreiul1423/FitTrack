using FitTrack.Api.Dtos;
using FitTrack.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitTrack.Api.Controllers;

[ApiController]
[Route("api/workouts")]
public class WorkoutsController : ControllerBase
{
    private readonly IWorkoutService _service;
    public WorkoutsController(IWorkoutService service) => _service = service;

    // GET /api/workouts
    [HttpGet]
    public async Task<ActionResult<List<WorkoutReadDto>>> GetAll()
        => Ok(await _service.GetAllAsync());

    // GET /api/workouts/5   <-- ASTA lipsea (de aici 404)
    [HttpGet("{id:int}")]
    public async Task<ActionResult<WorkoutDetailsDto>> GetById(int id)
    {
        var w = await _service.GetByIdAsync(id);
        return w is null ? NotFound() : Ok(w);
    }

    // POST /api/workouts
    [HttpPost]
    public async Task<ActionResult<WorkoutReadDto>> Create([FromBody] WorkoutCreateDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    // PUT /api/workouts/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] WorkoutUpdateDto dto)
        => (await _service.UpdateAsync(id, dto)) ? NoContent() : NotFound();

    // DELETE /api/workouts/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => (await _service.DeleteAsync(id)) ? NoContent() : NotFound();
}

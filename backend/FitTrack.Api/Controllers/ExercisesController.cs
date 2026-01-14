using FitTrack.Api.Dtos;
using FitTrack.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitTrack.Api.Controllers;

[ApiController]
[Route("api/exercises")]
public class ExercisesController : ControllerBase
{
    private readonly IExerciseService _service;
    public ExercisesController(IExerciseService service) => _service = service;

    // GET /api/exercises?workoutId=1
    [HttpGet]
    public async Task<ActionResult<List<ExerciseReadDto>>> GetAll([FromQuery] int? workoutId)
        => Ok(await _service.GetAllAsync(workoutId));

    // GET /api/exercises/5
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExerciseReadDto>> GetById(int id)
    {
        var ex = await _service.GetByIdAsync(id);
        return ex is null ? NotFound() : Ok(ex);
    }

    // POST /api/exercises
    [HttpPost]
    public async Task<ActionResult<ExerciseReadDto>> Create([FromBody] ExerciseCreateDto dto)
    {
        try
        {
            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            // aici intră când WorkoutId nu există
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT /api/exercises/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ExerciseUpdateDto dto)
        => (await _service.UpdateAsync(id, dto)) ? NoContent() : NotFound();

    // DELETE /api/exercises/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => (await _service.DeleteAsync(id)) ? NoContent() : NotFound();
}

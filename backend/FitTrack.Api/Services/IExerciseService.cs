using FitTrack.Api.Dtos;

namespace FitTrack.Api.Services;

public interface IExerciseService
{
    Task<List<ExerciseReadDto>> GetAllAsync(int? workoutId = null);
    Task<ExerciseReadDto?> GetByIdAsync(int id);
    Task<ExerciseReadDto> CreateAsync(ExerciseCreateDto dto);
    Task<bool> UpdateAsync(int id, ExerciseUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}

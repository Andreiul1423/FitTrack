using FitTrack.Api.Dtos;

namespace FitTrack.Api.Services;

public interface IWorkoutService
{
    Task<List<WorkoutReadDto>> GetAllAsync();
    Task<WorkoutDetailsDto?> GetByIdAsync(int id);
    Task<WorkoutReadDto> CreateAsync(WorkoutCreateDto dto);
    Task<bool> UpdateAsync(int id, WorkoutUpdateDto dto);
    Task<bool> DeleteAsync(int id);
}

namespace FitTrack.Api.Dtos;

public record WorkoutCreateDto(string Name, DateTime Date, string? Notes, string? Description);
public record WorkoutUpdateDto(string Name, DateTime Date, string? Notes, string? Description);
public record WorkoutReadDto(int Id, string Name, DateTime Date, string? Notes, string? Description);

public record WorkoutDetailsDto(
    int Id,
    string Name,
    DateTime Date,
    string? Notes,
    string? Description,
    List<ExerciseReadDto> Exercises
);

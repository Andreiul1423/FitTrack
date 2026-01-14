namespace FitTrack.Api.Dtos;

public record ExerciseCreateDto(int WorkoutId, string Name, int Sets, int Reps, double WeightKg, int RestSeconds);
public record ExerciseUpdateDto(string Name, int Sets, int Reps, double WeightKg, int RestSeconds);
public record ExerciseReadDto(int Id, int WorkoutId, string Name, int Sets, int Reps, double WeightKg, int RestSeconds);

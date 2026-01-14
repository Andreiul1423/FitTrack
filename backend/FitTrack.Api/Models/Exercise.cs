using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitTrack.Api.Models;

public class Exercise
{
    public int Id { get; set; }

    [ForeignKey(nameof(Workout))]
    public int WorkoutId { get; set; }

    public Workout? Workout { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;

    [Range(1, 100)]
    public int Sets { get; set; } = 3;

    [Range(1, 500)]
    public int Reps { get; set; } = 10;

    [Range(0, 500)]
    public double WeightKg { get; set; } = 0;

    [Range(0, 3600)]
    public int RestSeconds { get; set; } = 60;
}

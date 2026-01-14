using System.ComponentModel.DataAnnotations;

namespace FitTrack.Api.Models;

public class Workout
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = default!;

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [MaxLength(1000)]
    public string? Notes { get; set; }
    public string? Description { get; set; }
    public List<Exercise> Exercises { get; set; } = new();
}

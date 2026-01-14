using FitTrack.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Api.Data;

public class FitTrackDbContext : DbContext
{
    public FitTrackDbContext(DbContextOptions<FitTrackDbContext> options) : base(options) {}

    public DbSet<Workout> Workouts => Set<Workout>();
    public DbSet<Exercise> Exercises => Set<Exercise>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Workout>()
            .HasMany(w => w.Exercises)
            .WithOne(e => e.Workout!)
            .HasForeignKey(e => e.WorkoutId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

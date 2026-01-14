using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FitTrack.Api.Data;

public class FitTrackDbContextFactory : IDesignTimeDbContextFactory<FitTrackDbContext>
{
    public FitTrackDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FitTrackDbContext>();

        // pentru rulare locală (în proiect)
        optionsBuilder.UseSqlite("Data Source=fittrack.db");

        return new FitTrackDbContext(optionsBuilder.Options);
    }
}

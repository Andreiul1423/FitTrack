using FitTrack.Api.Data;
using FitTrack.Api.Mapping;
using FitTrack.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MappingProfile));

var conn = builder.Configuration.GetConnectionString("Default")
          ?? "Data Source=fittrack.db";
builder.Services.AddDbContext<FitTrackDbContext>(opt => opt.UseSqlite(conn));

builder.Services.AddScoped<IWorkoutService, WorkoutService>();
builder.Services.AddScoped<IExerciseService, ExerciseService>();

builder.Services.AddCors(opt =>
{
    opt.AddPolicy("AllowFrontend", p =>
        p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FitTrackDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");
app.MapControllers();

app.Run();

using AutoMapper;
using AutoMapper.QueryableExtensions;
using FitTrack.Api.Data;
using FitTrack.Api.Dtos;
using FitTrack.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Api.Services;

public class ExerciseService : IExerciseService
{
    private readonly FitTrackDbContext _db;
    private readonly IMapper _mapper;

    public ExerciseService(FitTrackDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<ExerciseReadDto>> GetAllAsync(int? workoutId = null)
    {
        var q = _db.Exercises.AsNoTracking().AsQueryable();
        if (workoutId is not null) q = q.Where(e => e.WorkoutId == workoutId);

        return await q.OrderBy(e => e.Id)
            .ProjectTo<ExerciseReadDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<ExerciseReadDto?> GetByIdAsync(int id)
    {
        var entity = await _db.Exercises.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
        return entity is null ? null : _mapper.Map<ExerciseReadDto>(entity);
    }

    public async Task<ExerciseReadDto> CreateAsync(ExerciseCreateDto dto)
    {
        // validare simplă: workout există
        var exists = await _db.Workouts.AnyAsync(w => w.Id == dto.WorkoutId);
        if (!exists) throw new ArgumentException("WorkoutId invalid.");

        var entity = _mapper.Map<Exercise>(dto);
        _db.Exercises.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.Map<ExerciseReadDto>(entity);
    }

    public async Task<bool> UpdateAsync(int id, ExerciseUpdateDto dto)
    {
        var entity = await _db.Exercises.FindAsync(id);
        if (entity is null) return false;

        _mapper.Map(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Exercises.FindAsync(id);
        if (entity is null) return false;

        _db.Exercises.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}

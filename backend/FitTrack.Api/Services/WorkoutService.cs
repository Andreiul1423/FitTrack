using AutoMapper;
using AutoMapper.QueryableExtensions;
using FitTrack.Api.Data;
using FitTrack.Api.Dtos;
using FitTrack.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FitTrack.Api.Services;

public class WorkoutService : IWorkoutService
{
    private readonly FitTrackDbContext _db;
    private readonly IMapper _mapper;

    public WorkoutService(FitTrackDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<WorkoutReadDto>> GetAllAsync()
        => await _db.Workouts
            .AsNoTracking()
            .OrderByDescending(w => w.Date)
            .ProjectTo<WorkoutReadDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

    public async Task<WorkoutDetailsDto?> GetByIdAsync(int id)
    {
        var workout = await _db.Workouts
            .AsNoTracking()
            .Include(w => w.Exercises)
            .FirstOrDefaultAsync(w => w.Id == id);

        return workout is null ? null : _mapper.Map<WorkoutDetailsDto>(workout);
    }

    public async Task<WorkoutReadDto> CreateAsync(WorkoutCreateDto dto)
    {
        var entity = _mapper.Map<Workout>(dto);
        _db.Workouts.Add(entity);
        await _db.SaveChangesAsync();
        return _mapper.Map<WorkoutReadDto>(entity);
    }

    public async Task<bool> UpdateAsync(int id, WorkoutUpdateDto dto)
    {
        var entity = await _db.Workouts.FindAsync(id);
        if (entity is null) return false;

        _mapper.Map(dto, entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _db.Workouts.FindAsync(id);
        if (entity is null) return false;

        _db.Workouts.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }
}

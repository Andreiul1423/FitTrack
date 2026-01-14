using AutoMapper;
using FitTrack.Api.Dtos;
using FitTrack.Api.Models;

namespace FitTrack.Api.Mapping;


public class MappingProfile : Profile

{
    public MappingProfile()
    {
        CreateMap<Workout, WorkoutReadDto>();
        CreateMap<Workout, WorkoutDetailsDto>();
        CreateMap<WorkoutCreateDto, Workout>();
        CreateMap<WorkoutUpdateDto, Workout>();

        CreateMap<Exercise, ExerciseReadDto>();
        CreateMap<ExerciseCreateDto, Exercise>();
        CreateMap<ExerciseUpdateDto, Exercise>();
    }
}

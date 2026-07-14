using AutoMapper;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;

namespace MusicStore.Services.Mappings;

public class GenreMapperProfile : Profile
{
    public GenreMapperProfile()
    {
        CreateMap<Genre, GenreResponseDto>();
        CreateMap<GenreRequestDto, Genre>();
    }
}

using AutoMapper;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Persistence.Projections;

namespace MusicStore.Services.Mappings;

public class ConcertMapperProfile : Profile
{
    public ConcertMapperProfile()
    {
        CreateMap<ConcertRequestDto, Concert>()
            .ForMember(dest => dest.DateEvent,
                opt => opt.MapFrom(src =>
                    src.DateEvent.ToDateTime(src.TimeEvent)))
            .ForMember(d => d.ImageUrl, options => options.Ignore());

        // Entity -> Response
        CreateMap<Concert, ConcertResponseDto>()
            .ForMember(dest => dest.Genre,
                opt => opt.MapFrom(src => src.Genre.Name))
            .ForMember(dest => dest.DateEvent,
                opt => opt.MapFrom(src =>
                    src.DateEvent.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.TimeEvent,
                opt => opt.MapFrom(src =>
                    src.DateEvent.ToString("HH:mm")))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(src =>
                    src.Status ? "Activo" : "Inactivo"));

        CreateMap<ConcertDetailView, ConcertResponseDto>()
            .ForMember(dest => dest.DateEvent,
                opt => opt.MapFrom(src =>
                    src.DateEvent.ToString("yyyy-MM-dd")))
            .ForMember(dest => dest.TimeEvent,
                opt => opt.MapFrom(src =>
                    src.DateEvent.ToString("HH:mm")));
    }
}

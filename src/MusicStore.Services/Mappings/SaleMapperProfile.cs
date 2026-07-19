using System.Globalization;
using AutoMapper;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Persistence.Projections;

namespace MusicStore.Services.Mappings;

public class SaleMapperProfile : Profile
{
    private static readonly CultureInfo culture = new("es-PE");

    public SaleMapperProfile()
    {
        CreateMap<SaleRequestDto, Sale>()
            .ForMember(d => d.Quantity, o => o.MapFrom(x => x.TicketsQuantity));

        CreateMap<SaleReportView, SaleReportResponseDto>();

        CreateMap<Sale, SaleResponseDto>()
            .ForMember(d => d.SaleId, o => o.MapFrom(x => x.Id))
            .ForMember(d => d.Email, o => o.MapFrom(x => x.Customer.Email))
            .ForMember(d => d.Genre, o => o.MapFrom(x => x.Concert.Genre.Name))
            .ForMember(d => d.ImageUrl, o => o.MapFrom(x => x.Concert.ImageUrl))
            .ForMember(d => d.Title, o => o.MapFrom(x => x.Concert.Title))
            .ForMember(d => d.FullName, o => o.MapFrom(x => x.Customer.FullName))
            .ForMember(d => d.DateEvent, o => o.MapFrom(x => x.Concert.DateEvent.ToString("D", culture)))
            .ForMember(d => d.TimeEvent, o => o.MapFrom(x => x.Concert.DateEvent.ToString("T", culture)))
            .ForMember(d => d.SaleDate, o => o.MapFrom(x => x.SaleDate.ToString("dd/MM/yyyy HH:mm", culture)));
    }
}
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStore.Dto.Common;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Repositories;
using MusicStore.Services.Exceptions;
using MusicStore.Services.Helpers;

namespace MusicStore.Services;

public class SaleService(
    ISaleRepository repository,
    IConcertRepository concertRepository,
    ICustomerRepository customerRepository,
    IUnitOfWork unitOfWork,
    ILogger<SaleService> logger,
    IMapper mapper
) : ISaleService
{
    public async Task<int> AddAsync(string email, SaleRequestDto request)
    {
        var customer = await customerRepository.GetByEmailAsync(email);
        if (customer is null)
        {
            //throw new BusinessException($"La cuenta {email} no está registrada como cliente.");
            customer = new Customer()
            {
                Id = 0, // Valor predeterminado, será asignado por la base de datos al guardar
                Email = email,
                FullName = $"Usuario {email}"
            };
            customer = await customerRepository.AddAsync(customer);
        }

        var concert = await concertRepository.GetByIdAsync(request.ConcertId);
        if (concert is null || !concert.Status)
            throw new NotFoundException($"El concierto {request.ConcertId} no existe.");

        if (concert.Finalized)
            throw new BusinessException("El concierto ya finalizó.");

        if (DateTime.Today > concert.DateEvent)
            throw new BusinessException("No se pueden comprar tickets de eventos pasados.");

        var entity = mapper.Map<Sale>(request);

        entity.Customer = customer;
        entity.ConcertId = concert.Id;
        entity.SaleDate = DateTime.UtcNow;
        entity.Total = entity.Quantity * concert.UnitPrice;
        entity.OperationCode = Guid.NewGuid().ToString();

        await repository.AddAsync(entity);
        await unitOfWork.CommitAsync();

        logger.LogInformation("Venta creada correctamente para {Email}", email);

        return entity.Id;
    }

    public async Task<SaleResponseDto> GetAsync(int id)
    {
        var sale = await repository
            .Query()
            .Include(x => x.Customer)
            .Include(x => x.Concert)
            .ThenInclude(x => x.Genre)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (sale is null)
            throw new NotFoundException($"No se encontró la venta {id}");

        return mapper.Map<SaleResponseDto>(sale);
    }

    public async Task<PaginatedResult<SaleResponseDto>> GetByDateAsync(
        DateTime? start,
        DateTime? end,
        int page,
        int pageSize)
    {
        var query = repository
            .Query()
            .Include(x => x.Customer)
            .Include(x => x.Concert)
            .ThenInclude(x => x.Genre)
            .AsQueryable();

        if (start.HasValue)
            query = query.Where(x => x.SaleDate >= start.Value);

        if (end.HasValue)
            query = query.Where(x => x.SaleDate <= end.Value);

        var result = await query.OrderByDescending(x => x.SaleDate).ToPaginatedAsync(page, pageSize);

        return new PaginatedResult<SaleResponseDto>
        {
            Data = mapper.Map<IReadOnlyList<SaleResponseDto>>(result.Data),
            Meta = result.Meta
        };
    }

    public async Task<PaginatedResult<SaleResponseDto>> GetByCustomerAsync(
        string email,
        bool includeAll,
        string? title,
        int page,
        int pageSize)
    {
        IQueryable<Sale> query = repository.Query();

        if (!includeAll) //si es admin retorna todas las ventas, no solo las de su usuario
            query = query.Where(x => x.Customer.Email == email);

        if (!string.IsNullOrWhiteSpace(title))
            query = query.Where(x => x.Concert.Title.Contains(title));

        query = query
            .Include(x => x.Customer)
            .Include(x => x.Concert)
            .ThenInclude(x => x.Genre);

        var result = await query.OrderByDescending(x => x.SaleDate).ToPaginatedAsync(page, pageSize);

        return new PaginatedResult<SaleResponseDto>
        {
            Data = mapper.Map<IReadOnlyList<SaleResponseDto>>(result.Data),
            Meta = result.Meta
        };
    }

    public async Task<ICollection<SaleReportResponseDto>> GetSaleReportAsync(
        DateTime? dateStart,
        DateTime? dateEnd)
    {
        var data = await repository.GetSaleReportAsync(dateStart, dateEnd);
        return mapper.Map<ICollection<SaleReportResponseDto>>(data);
    }
}
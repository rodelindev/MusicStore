using MusicStore.Dto.Common;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services;

public interface ISaleService
{
    Task<int> AddAsync(string email, SaleRequestDto request);
    Task<SaleResponseDto> GetAsync(int id);
    Task<PaginatedResult<SaleResponseDto>> GetByDateAsync(DateTime? start, DateTime? end, int page, int pageSize);
    Task<PaginatedResult<SaleResponseDto>> GetByCustomerAsync(string email, bool includeAll, string? title, int page, int pageSize);
    Task<ICollection<SaleReportResponseDto>> GetSaleReportAsync(DateTime? dateStart, DateTime? dateEnd);
}
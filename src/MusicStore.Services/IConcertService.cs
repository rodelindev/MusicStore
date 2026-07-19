using MusicStore.Dto.Common;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services;

public interface IConcertService
{
    Task<PaginatedResult<ConcertResponseDto>> GetAsync(string? title, int page, int pageSize);
    Task<IReadOnlyList<ConcertResponseDto>> GetUpcomingAsync();
    Task<ConcertResponseDto> GetAsync(int id);
    Task<ConcertResponseDto> AddAsync(ConcertRequestDto request);
    Task<ConcertResponseDto> UpdateAsync(int id, ConcertRequestDto request);
    Task DeleteAsync(int id);
    Task FinalizeAsync(int id);
}
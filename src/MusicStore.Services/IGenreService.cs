using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services;

public interface IGenreService
{
    Task<IReadOnlyList<GenreResponseDto>> GetAsync();
    Task<GenreResponseDto> GetByIdAsync(int id);
    Task<GenreResponseDto> AddAsync(GenreRequestDto request);
    Task<GenreResponseDto> UpdateAsync(int id, GenreRequestDto request);
    Task DeleteAsync(int id);
}

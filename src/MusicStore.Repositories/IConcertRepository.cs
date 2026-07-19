using MusicStore.Entities;
using MusicStore.Persistence.Projections;

namespace MusicStore.Repositories;

public interface IConcertRepository : IRepositoryBase<Concert, int>
{
    Task<ICollection<ConcertDetailView>> GetDetailAsync(string? title);
    Task<ICollection<ConcertDetailView>> GetDetailRawAsync(string? title);
}
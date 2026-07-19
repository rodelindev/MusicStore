using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MusicStore.Entities;
using MusicStore.Persistence;
using MusicStore.Persistence.Projections;

namespace MusicStore.Repositories;

public class ConcertRepository : RepositoryBase<Concert, int>, IConcertRepository
{
    public ConcertRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ICollection<ConcertDetailView>> GetDetailAsync(string? title)
    {
        var query = _context.Set<Concert>()
            .IgnoreQueryFilters()
            .Where(x => x.Status)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(x => x.Title.Contains(title));
        }

        return await query
            .OrderBy(x => x.Id)
            .Select(x => new ConcertDetailView
            {
                Id = x.Id,
                Title = x.Title,
                Description = x.Description,
                ExtendedDescription = x.ExtendedDescription,
                Place = x.Place,
                UnitPrice = x.UnitPrice,
                GenreId = x.GenreId,
                Genre = x.Genre.Name,
                DateEvent = x.DateEvent,
                ImageUrl = x.ImageUrl,
                Capacity = x.Capacity,
                Finalized = x.Finalized,
                Status = x.Status ? "Activo" : "Inactivo"
            })
            .ToListAsync();
    }

    public async Task<ICollection<ConcertDetailView>> GetDetailRawAsync(string? title)
    {
        var sql = @"
        SELECT
            c.Id,
            c.Title,
            c.Description,
            c.ExtendedDescription,
            c.Place,
            c.UnitPrice,
            c.GenreId,
            g.Name AS Genre,
            c.DateEvent,
            c.ImageUrl,
            c.Capacity,
            c.Finalized,
            CASE
                WHEN c.Status = 1 THEN 'Activo'
                ELSE 'Inactivo'
            END AS Status
        FROM MusicStore.Concert c
        INNER JOIN MusicStore.Genre g ON g.Id = c.GenreId
        WHERE c.Status = 1 AND
            (@title IS NULL OR c.Title LIKE '%' + @title + '%')
        ORDER BY c.Id";

        

        var parameter = new SqlParameter("@title", string.IsNullOrWhiteSpace(title) ? DBNull.Value : title);

        return await _context
            .Set<ConcertDetailView>()
            .FromSqlRaw(sql, parameter)
            .AsNoTracking()
            .ToListAsync();
    }
}
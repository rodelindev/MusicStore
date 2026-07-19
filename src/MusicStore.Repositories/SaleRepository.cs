using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MusicStore.Entities;
using MusicStore.Persistence;
using MusicStore.Persistence.Projections;

namespace MusicStore.Repositories;

public class SaleRepository : RepositoryBase<Sale, int>, ISaleRepository
{
    public SaleRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ICollection<SaleReportView>> GetSaleReportAsync(DateTime? dateStart, DateTime? dateEnd)
    {
        var query = _context.Database
            .SqlQueryRaw<SaleReportView>(
                @"SELECT 
                    c.Id AS ConcertId,
                    c.Title AS ConcertName, 
                    SUM(s.Total) AS Total
                    FROM MusicStore.Sale s
                    INNER JOIN MusicStore.Concert c ON c.Id = s.ConcertId
                      WHERE 
                        (@dateStart IS NULL OR s.SaleDate >= @dateStart)
                        AND (@dateEnd IS NULL OR s.SaleDate <= @dateEnd)
                    GROUP BY c.Id, c.Title",
                new SqlParameter("@dateStart", dateStart ?? (object)DBNull.Value),
                new SqlParameter("@dateEnd", dateEnd ?? (object)DBNull.Value)
            );

        return await query.ToListAsync();
    }
}

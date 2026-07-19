using MusicStore.Entities;
using MusicStore.Persistence.Projections;

namespace MusicStore.Repositories;

public interface ISaleRepository : IRepositoryBase<Sale, int>
{
    Task<ICollection<SaleReportView>> GetSaleReportAsync(DateTime? dateStart, DateTime? dateEnd);
}

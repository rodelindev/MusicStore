namespace MusicStore.Persistence.Projections;

public record SaleReportView
{
    public int ConcertId { get; set; }
    public string ConcertName { get; set; } = default!;
    public decimal Total { get; set; }
}

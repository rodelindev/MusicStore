namespace MusicStore.Dto.Response;

public class SaleReportResponseDto
{
    public int ConcertId { get; set; }
    public string ConcertName { get; set; } = default!;
    public decimal Total { get; set; }
}
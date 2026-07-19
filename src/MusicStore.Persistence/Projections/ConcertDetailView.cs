namespace MusicStore.Persistence.Projections;

public class ConcertDetailView
{
    public int Id { get; init; }
    public string Title { get; init; } = default!;
    public string Description { get; init; } = default!;
    public string ExtendedDescription { get; init; } = default!;
    public string Place { get; init; } = default!;
    public decimal UnitPrice { get; init; }
    public int GenreId { get; init; }
    public string Genre { get; init; } = default!;
    public DateTime DateEvent { get; init; }
    public string? ImageUrl { get; init; }
    public int Capacity { get; init; }
    public bool Finalized { get; init; }
    public string Status { get; init; } = default!;
}
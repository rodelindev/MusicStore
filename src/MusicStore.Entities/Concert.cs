namespace MusicStore.Entities;

public class Concert : EntityBase<int>
{
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ExtendedDescription { get; set; } = default!;
    public string Place { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int GenreId { get; set; }
    public DateTime DateEvent { get; set; }
    public string? ImageUrl { get; set; }
    public int Capacity { get; set; }
    public bool Finalized { get; set; }
    //Navigation properties
    public virtual Genre Genre { get; set; } = default!;
}
namespace MusicStore.Entities;

public class Sale : EntityBase<int>
{
    public int CustomerId { get; set; }
    public int ConcertId { get; set; }
    public DateTime SaleDate { get; set; }
    public string OperationCode { get; set; } = default!;
    public decimal Total { get; set; }
    public int Quantity { get; set; }

    //navigation properties
    public Customer Customer { get; set; } = default!;
    public Concert Concert { get; set; } = default!;
}

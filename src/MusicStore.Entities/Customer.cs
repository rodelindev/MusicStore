namespace MusicStore.Entities;

public class Customer : EntityBase<int>
{
    public string Email { get; set; } = default!;
    public string FullName { get; set; } = default!;
}
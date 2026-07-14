namespace MusicStore.Entities;

public class EntityBase<TKey>
{
    public required TKey Id { get; set; }
    public bool Status { get; set; } = true;
}
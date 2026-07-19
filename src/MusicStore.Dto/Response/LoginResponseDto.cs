namespace MusicStore.Dto.Response;

public class LoginResponseDto
{
    public string Token { get; set; } = default!;
    public DateTime ExpirationDate { get; set; }
}
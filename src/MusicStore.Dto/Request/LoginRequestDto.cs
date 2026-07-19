namespace MusicStore.Dto.Request;

public class LoginRequestDto
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
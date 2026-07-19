using MusicStore.Dto.Request;
using MusicStore.Dto.Response;

namespace MusicStore.Services;

public interface IUserService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task RequestTokenToResetPasswordAsync(ResetPasswordRequestDto request);
    Task ResetPasswordAsync(string email, string token, string newPassword);
    Task ChangePasswordAsync(string email, string oldPassword, string newPassword);
}
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MusicStore.Dto.Request;
using MusicStore.Dto.Response;
using MusicStore.Entities;
using MusicStore.Services.Exceptions;

namespace MusicStore.Services;

public class UserService(
    UserManager<MusicStoreUserIdentity> userManager,
    SignInManager<MusicStoreUserIdentity> signInManager,
    IOptions<AppSettings> options,
    IEmailService emailService,
    ILogger<UserService> logger
) : IUserService
{
    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var user = new MusicStoreUserIdentity
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Age = request.Age,
            DocumentNumber = request.DocumentNumber,
            DocumentType = (DocumentTypeEnum)request.DocumentType,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.ConfirmPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            logger.LogWarning("Registration failed: {Errors}", errors);
            throw new BusinessException(errors);
        }

        await userManager.AddToRoleAsync(user, "Customer");

        logger.LogInformation("User registered successfully: {Email}", request.Email);

        var tokenResponse = await BuildToken(user);

        return new RegisterResponseDto
        {
            UserId = user.Id,
            Token = tokenResponse.Token,
            ExpirationDate = tokenResponse.ExpirationDate
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var result = await signInManager.PasswordSignInAsync(
            request.Username,
            request.Password,
            isPersistent: false,
            lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            logger.LogWarning("Login failed for: {Username}", request.Username);
            throw new ArgumentException("Invalid credentials");
        }

        var user = await userManager.FindByEmailAsync(request.Username);
        logger.LogInformation("User logged in: {Email}", request.Username);

        return await BuildToken(user!);
    }

    public async Task RequestTokenToResetPasswordAsync(ResetPasswordRequestDto request)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            logger.LogWarning("Password reset requested for non-existent email: {Email}", request.Email);
            return;
        }

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        await emailService.SendEmailAsync(request.Email, "Password Reset",
            $"<p>Use this token to reset your password:</p><p><strong>{token}</strong></p>");
    }

    public async Task ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new BusinessException(errors);
        }

        logger.LogInformation("Password reset for: {Email}", email);
    }

    public async Task ChangePasswordAsync(string email, string oldPassword, string newPassword)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            throw new KeyNotFoundException("User not found");
        }

        var result = await userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            throw new BusinessException(errors);
        }

        logger.LogInformation("Password changed for: {Email}", email);
    }

    private async Task<LoginResponseDto> BuildToken(MusicStoreUserIdentity user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Jwt.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddSeconds(options.Value.Jwt.LifetimeInSeconds);

        var token = new JwtSecurityToken(
            issuer: options.Value.Jwt.Issuer,
            audience: options.Value.Jwt.Audience,
            claims: claims,
            signingCredentials: credentials,
            expires: expiration);

        return new LoginResponseDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpirationDate = expiration
        };
    }
}
using ECommerce.Application.Common.Interfaces;
using ECommerce.Domain.Identity.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace ECommerce.Application.Identity.Commands;

public class RegisterHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IApplicationDbContext _db;
    public RegisterHandler(IApplicationDbContext db) => _db = db;

    public async Task<Guid> Handle(RegisterCommand cmd, CancellationToken ct)
    {
        var exists = await _db.Users.AnyAsync(u => u.Email == cmd.Email.ToLowerInvariant(), ct);
        if (exists) throw new InvalidOperationException("Email already registered.");
        var hash = HashPassword(cmd.Password);
        var user = AppUser.Create(cmd.FirstName, cmd.LastName, cmd.Email, hash);
        await _db.Users.AddAsync(user, ct);
        await _db.SaveChangesAsync(ct);
        return user.Id;
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }
}

public class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IApplicationDbContext _db;
    private readonly IJwtService _jwt;
    public LoginHandler(IApplicationDbContext db, IJwtService jwt) { _db = db; _jwt = jwt; }

    public async Task<LoginResult> Handle(LoginCommand cmd, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == cmd.Email.ToLowerInvariant(), ct)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");
        if (!VerifyPassword(cmd.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");
        var accessToken = _jwt.GenerateAccessToken(user);
        var refreshToken = _jwt.GenerateRefreshToken();
        var refreshExpiry = DateTime.UtcNow.AddDays(7);
        user.SetRefreshToken(refreshToken, refreshExpiry);
        await _db.SaveChangesAsync(ct);
        return new LoginResult(accessToken, refreshToken, refreshExpiry, user.FullName, user.Role.ToString());
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);
        var computed = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(hash, computed);
    }
}

public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResult>
{
    private readonly IApplicationDbContext _db;
    private readonly IJwtService _jwt;
    public RefreshTokenHandler(IApplicationDbContext db, IJwtService jwt) { _db = db; _jwt = jwt; }

    public async Task<LoginResult> Handle(RefreshTokenCommand cmd, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.RefreshToken == cmd.RefreshToken, ct)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");
        if (!user.IsRefreshTokenValid(cmd.RefreshToken))
            throw new UnauthorizedAccessException("Refresh token expired.");
        var newAccessToken = _jwt.GenerateAccessToken(user);
        var newRefreshToken = _jwt.GenerateRefreshToken();
        var newExpiry = DateTime.UtcNow.AddDays(7);
        user.SetRefreshToken(newRefreshToken, newExpiry);
        await _db.SaveChangesAsync(ct);
        return new LoginResult(newAccessToken, newRefreshToken, newExpiry, user.FullName, user.Role.ToString());
    }
}

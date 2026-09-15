using Microsoft.EntityFrameworkCore;
using OrderManagementApi.Data;
using OrderManagementApi.Models.DTOs;
using OrderManagementApi.Models.Entities;

namespace OrderManagementApi.Services;

public class AuthService : IAuthService
{
    private readonly OrderDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly IJwtService _jwtService;

    public AuthService(
        OrderDbContext context,
        IPasswordService passwordService,
        IJwtService jwtService)
    {
        _context = context;
        _passwordService = passwordService;
        _jwtService = jwtService;
    }

    public async Task<bool> RegisterAsync(RegisterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Username))
        {
            throw new ArgumentException("Username is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            throw new ArgumentException("Password is required.");
        }

        var existingUser = await _context.AppUsers
            .AnyAsync(u => u.Username == request.Username);

        if (existingUser)
        {
            throw new ArgumentException("Username already exists.");
        }

        var user = new AppUser
        {
            Username = request.Username,
            Role = "Salesperson"
        };

        user.PasswordHash = _passwordService.HashPassword(request.Password);

        _context.AppUsers.Add(user);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<string?> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.AppUsers
            .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user is null)
        {
            return null;
        }

        var passwordValid = _passwordService.VerifyPassword(
            request.Password,
            user.PasswordHash);

        if (!passwordValid)
        {
            return null;
        }

        return _jwtService.GenerateToken(user);
    }
}
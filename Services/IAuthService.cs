using OrderManagementApi.Models.DTOs;

namespace OrderManagementApi.Services;

public interface IAuthService
{
    Task<bool> RegisterAsync(RegisterRequestDto request);

    Task<string?> LoginAsync(LoginRequestDto request);
}
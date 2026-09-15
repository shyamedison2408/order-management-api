using OrderManagementApi.Models.Entities;

namespace OrderManagementApi.Services;

public interface IJwtService
{
    string GenerateToken(AppUser user);
}
using MyCms.Api.Models;

namespace MyCms.Api.Services;

public interface IJwtService
{
    string GenerateToken(User user, IEnumerable<string> roles);
    bool ValidateToken(string token);
}
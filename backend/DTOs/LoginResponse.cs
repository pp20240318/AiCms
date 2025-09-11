namespace MyCms.Api.DTOs;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public UserDto User { get; set; } = new();
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
using Inventory.Application.DTOs;

namespace Inventory.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
}

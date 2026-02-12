using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check if user already exists
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("User with this email already exists");
        }

        // Hash password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // Create user
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            Role = registerDto.Role,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Generate token
        var token = _jwtService.GenerateToken(user.Id, user.Username, user.Email, user.Role);

        return new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Get user by email
        var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Verify password
        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password");
        }

        // Generate token
        var token = _jwtService.GenerateToken(user.Id, user.Username, user.Email, user.Role);

        return new LoginResponseDto
        {
            Token = token,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };
    }
}

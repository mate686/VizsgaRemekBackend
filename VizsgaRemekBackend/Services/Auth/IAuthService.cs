using Microsoft.AspNetCore.Identity;
using VizsgaRemekBackend.Dtos.AuthDtos;

namespace VizsgaRemekBackend.Services.Auth
{
    public interface IAuthService
    {
        public Task<string> RegisterAsync(RegisterDto dto);
        public Task<string?> LoginAsync(LoginDto ldto);
        Task<ProfileDto?> GetProfileAsync(string userId);
        Task<(ProfileDto? profile, string? error)> UpdateProfileAsync(string userId, UpdateProfileDto dto);
    }
}

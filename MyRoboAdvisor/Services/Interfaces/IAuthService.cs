using MyRoboAdvisor.Models;

namespace MyRoboAdvisor.Services.Interfaces;

public interface IAuthService
{
    Task<Response<string>> Register(RegisterUserDto userDto);
    Task<Response<LoginResponseDto>> Login(LoginDto loginDto);
}
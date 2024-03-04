using Microsoft.AspNetCore.Mvc;
using MyRoboAdvisor.Models;
using MyRoboAdvisor.Services.Interfaces;

namespace MyRoboAdvisor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Response<LoginResponseDto>>> Register([FromBody] RegisterUserDto model)
    {
        var result = await _authService.Register(model);
        return StatusCode(result.StatusCode, result);
    }


    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Response<string>>> Login([FromBody] LoginDto model)
    {
        var result = await _authService.Login(model);
        return StatusCode(result.StatusCode, result);
    }
}
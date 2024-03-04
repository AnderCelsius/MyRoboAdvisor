using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyRoboAdvisor.Database.Repositories;
using MyRoboAdvisor.Domain.Entities;
using MyRoboAdvisor.Models;
using MyRoboAdvisor.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace MyRoboAdvisor.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepository<ApplicationUser> _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            IGenericRepository<ApplicationUser> userRepository,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _userRepository = userRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Logs in a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response<LoginResponseDto>> Login(LoginDto model)
        {
            var validityResult = await ValidateUser(model);

            if (!validityResult.Succeeded)
            {
                return Response<LoginResponseDto>.Fail(validityResult.Message, validityResult.StatusCode);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null) return Response<LoginResponseDto>.Fail("user not found");

            var result = new LoginResponseDto()
            {
                Id = user.Id,
                Token = await GenerateToken(user),
            };

            await _userManager.UpdateAsync(user);

            return Response<LoginResponseDto>.Success("Login Successfully", result);
        }

        /// <summary>
        /// Registers a new user and sends a confirmation link to the user's email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<Response<string>> Register(RegisterUserDto model)
        {
            var user = new ApplicationUser()
            {
                // Todo: map user properties.
                EmailConfirmed = true, // Since there is no email service
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return Response<string>.Fail(GetErrors(result), (int)HttpStatusCode.BadRequest);

            await _userRepository.InsertAsync(user);
            await _userRepository.SaveAsync();
            return Response<string>.Success("User created successfully", user.Id, (int)HttpStatusCode.Created);

        }

        /// <summary>
        /// Validates a user
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Returns true if the user exists</returns>
        private async Task<Response<bool>> ValidateUser(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Response<bool>.Fail("Invalid Credentials", (int)HttpStatusCode.BadRequest);
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return Response<bool>.Fail("Account not activated", (int)HttpStatusCode.Forbidden);
            }

            return Response<bool>.Success("Succeeded", true);
        }

        /// <summary>
        /// Stringify and returns all the identity errors
        /// </summary>
        /// <param name="result"></param>
        /// <returns>Identity Errors</returns>
        private static string GetErrors(IdentityResult result)
        {
            return result.Errors.Aggregate(string.Empty, (current, err) => current + err.Description + "\n");
        }

        /// <summary>
        /// Generates a bearer JWT token for a logged user which is used for Authorization
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> GenerateToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.GivenName, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
            };

            //Gets the roles of the logged in user and adds it to Claims
            var roles = await _userManager.GetRolesAsync(user);
            authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

            // Specifying JWTSecurityToken Parameters
            var token = new JwtSecurityToken
            (audience: _configuration["JwtSettings:Audience"],
                issuer: _configuration["JwtSettings:Issuer"],
                claims: authClaims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

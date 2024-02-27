using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Servify.Data;
using Servify.DTOs;
using Servify.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Servify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ServifyDbContext _context;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ServifyDbContext context, IConfiguration config, UserManager<User> userManager, ILogger<AuthController> logger)
        {
            _context = context;
            _config = config;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .FirstOrDefault();
                _logger.LogError($"Enter a valid inputs");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = errorMessage ?? "Invalid employee data",
                });
            }
            var userExist = await _userManager.FindByNameAsync(registerUserDto.Username);
            if (userExist != null)
            {
                _logger.LogError($"Faild to create user already exists");
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists" });
            }

            var user = new User()
            {
                Email = registerUserDto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUserDto.Username
            };

            var result = await _userManager.CreateAsync(user, registerUserDto.Password);
            if (!result.Succeeded)
            {
                _logger.LogError($"Internal Error: Faild to create user");

                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Faild to create a user." });
            }

            _logger.LogInformation($"User ${user.UserName} Created Successfully");
            return Ok(new Response { Status = "Success", Message = "User Created Successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
        {
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.Values
                       .SelectMany(v => v.Errors)
                       .Select(e => e.ErrorMessage)
                       .FirstOrDefault();
                _logger.LogError($"Enter a valid inputs");
                return BadRequest(new Response
                {
                    Status = "Error",
                    Message = errorMessage ?? "Invalid employee data",
                });
            }

            var user = await _userManager.FindByNameAsync(loginUserDto.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, loginUserDto.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, loginUserDto.Username),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

                var token = new JwtSecurityToken(
                        issuer: _config["Jwt:Issuer"],
                        audience: _config["Jwt:Audience"],
                        expires: DateTime.Now.AddHours(2),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                _logger.LogInformation($"User ${user.UserName} Login Successfully");
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    User = user.UserName
                });
            }
            return Unauthorized();
        }

    }
}

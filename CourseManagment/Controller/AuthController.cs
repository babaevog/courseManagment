using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CourseManagment.Data;
using CourseManagment.DTOs;
using CourseManagment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CourseManagment.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly ApplicationDBContext _context;
		private readonly PasswordHasher<User> _hasher;
		private readonly IConfiguration _configuration;
		private string GenerateRefreshToken()
		{
			var randomBytes = new byte[32];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomBytes);
			return Convert.ToBase64String(randomBytes);
		}

		public AuthController(ApplicationDBContext context, IConfiguration configuration)
		{
			_context = context;
			_hasher = new PasswordHasher<User>();
			_configuration = configuration;

		}
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (_context.Users.Any(u => u.Email == dto.Email))
			{
				return BadRequest("Email is already registered");
			}
			if (_context.Users.Any(u => u.Username == dto.UserName))
			{
				return BadRequest("Username is already registered");
			}

			var user = new User
			{
				Email = dto.Email,
				Username = dto.UserName,
				Role = dto.Role
			};
			user.PasswordHash = _hasher.HashPassword(user, dto.Password);
			_context.Users.Add(user);
			await _context.SaveChangesAsync();
			return Ok("User register successfull");
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDTO dto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
			if (user == null)
				return Unauthorized("Пользователь не найден");

			var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
			if (result != PasswordVerificationResult.Success)
				return Unauthorized("Неверный пароль");

			// Генерация JWT
			var jwtConfig = _configuration.GetSection("Jwt");
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
	new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
	new Claim(ClaimTypes.Name, user.Username),
	new Claim(ClaimTypes.Role, user.Role)
  };

			var token = new JwtSecurityToken(
			  issuer: jwtConfig["Issuer"],
			  audience: jwtConfig["Audience"],
			  claims: claims,
			  expires: DateTime.UtcNow.AddMinutes(double.Parse(jwtConfig["ExpiresInMinutes"])),
			  signingCredentials: creds
			);

			var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
			return Ok(new { token = tokenString });

		}
	}
}

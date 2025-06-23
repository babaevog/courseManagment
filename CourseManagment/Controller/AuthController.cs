using CourseManagment.Data;
using CourseManagment.DTOs;
using CourseManagment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CourseManagment.Controller
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly ApplicationDBContext _context;
		private readonly PasswordHasher<User> _hasher;

		public AuthController(ApplicationDBContext context)
		{
			_context = context;
			_hasher = new PasswordHasher<User>();

		}
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if(_context.Users.Any(u => u.Email == dto.Email))
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
				Username = dto.UserName
			};
			user.PasswordHash = _hasher.HashPassword(user, dto.Password);
			_context.Users.Add(user);
			await _context.SaveChangesAsync();
			return Ok("User register successfull");
		}
	}
}

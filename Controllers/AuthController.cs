using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimpleNote.Data.Repository;
using SimpleNote.DTO;
using SimpleNote.Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SimpleNote.Controllers
{
	[Route("api/[controller]")]

	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IConfiguration _configuration;
		public AuthController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[HttpPost("register")]
		public async Task<ActionResult<User>> PostAsync(UserDto request)
		{
			await Task.Yield();
			CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
			var user = new User()
			{
				Id = System.Guid.NewGuid().ToString(),
				UserName = request.UserName,
				DisplayName = request.Name,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt
			};

			var userRepo = new UserRepository(_configuration);

			if (await userRepo.CreateAsync(user))
			{
				return Ok();
			}
			else
			{
				return Problem("Error al crear el usuario");
			}
		}

		private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512())
			{
				passwordSalt = hmac.Key;
				passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
			}
		}

		[HttpPost("login")]
		public async Task<ActionResult> Login(UserDto request)
		{
			var user = await new UserRepository(_configuration).FindByUserNameAsync(request.UserName);
			if (user == null)
			{
				return BadRequest("User not found");
			}

			if (!VerifyPasswordHash(user, request.Password, user.PasswordHash, user.PasswordSalt))
			{
				BadRequest("Wrong password");
			}

			string token = CreateToken(user);
			return Ok(token);
		}

		private bool VerifyPasswordHash(User user, string password, byte[] passwordHash, byte[] passwordSalt)
		{
			using (var hmac = new HMACSHA512(user.PasswordSalt))
			{
				var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
				return computeHash.SequenceEqual(passwordHash);
			}
		}

		private string CreateToken(User user)
		{
			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(ClaimTypes.Role, "Admin")
			};

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
				_configuration.GetSection("AppSettings:Token").Value));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.Now.AddDays(1),
				signingCredentials: creds);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);

			return jwt;
		}
	}
}
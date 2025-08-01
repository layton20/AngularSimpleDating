using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly AppDbContext __Context;
        private readonly ITokenService __TokenService;

        public AccountController(AppDbContext context, ITokenService tokenService)
        {
            __Context = context;
            __TokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> RegisterAsync(RegisterDto registerDto)
        {
            if (await EmailExistsAsync(registerDto.Email))
            {
                return BadRequest("Email is already in use.");
            }

            using HMACSHA512 _HMAC = new();

            AppUser _User = new()
            {
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                PasswordHash = _HMAC.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = _HMAC.Key
            };

            __Context.Users.Add(_User);
            await __Context.SaveChangesAsync();

            return _User.ToDto(__TokenService);
        }

        private async Task<bool> EmailExistsAsync(string email)
        {
            return await __Context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> LoginAsync(LoginDto loginDto)
        {
            AppUser? _User = await __Context.Users.FirstOrDefaultAsync(x => x.Email == loginDto.Email);

            if (_User == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            using HMACSHA512 _HMAC = new(_User.PasswordSalt);

            byte[] _ComputedHash = _HMAC.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < _ComputedHash.Length; i++)
            {
                if (_ComputedHash[i] != _User.PasswordHash[i])
                {
                    return Unauthorized("Invalid email or password.");
                }
            }

            return _User.ToDto(__TokenService);
        }
    }
}

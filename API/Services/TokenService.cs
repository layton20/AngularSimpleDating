using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration __Configuration;

    public TokenService(IConfiguration configuration)
    {
        __Configuration = configuration;
    }

    public string CreateToken(AppUser user)
    {
        string _TokenKey = __Configuration["TokenKey"] ?? throw new Exception("Cannot get token key.");

        if (_TokenKey.Length < 64)
        {
            throw new ArgumentException("Token key must be at least 64 characters long.");
        }

        SymmetricSecurityKey _Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_TokenKey));

        List<Claim> _Claims =
        [
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.NameIdentifier, user.Id)
        ];

        SigningCredentials _Credentials = new(_Key, SecurityAlgorithms.HmacSha512Signature);
        JwtSecurityTokenHandler _TokenHandler = new();
        SecurityToken _JWTToken = _TokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(_Claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = _Credentials
        });

        return _TokenHandler.WriteToken(_JWTToken);
    }
}

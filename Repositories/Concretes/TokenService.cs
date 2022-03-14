using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using forfarmingcase.Repositories.Interfaces;
using forfarmingcase.ViewModels;
using Microsoft.IdentityModel.Tokens;

public class TokenService : ITokenService
{
  private const double EXPIRY_DURATION_MINUTES = 60;

  public string BuildToken(string key, string issuer, string audience, LoginVM loginVM)
  {
    var claims = new[] {
            new Claim(ClaimTypes.Name, loginVM.Username),
            new Claim(ClaimTypes.NameIdentifier,
            Guid.NewGuid().ToString())
        };

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
    var tokenDescriptor = new JwtSecurityToken(issuer, audience, claims,
        expires: DateTime.Now.AddMinutes(EXPIRY_DURATION_MINUTES), signingCredentials: credentials);
    return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
  }
}

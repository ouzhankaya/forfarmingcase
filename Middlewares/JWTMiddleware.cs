using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using forfarmingcase.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace forfarmingcase.Middlewares
{

    public class JWTMiddleware
    {
      private readonly RequestDelegate _next;
      private readonly IConfiguration _configuration;
      private readonly IUserRepository _userRepository;

      public JWTMiddleware(RequestDelegate next, IConfiguration configuration, IUserRepository userRepository)
      {
        _next = next;
        _configuration = configuration;
      _userRepository = userRepository;
      }

      public async Task Invoke(HttpContext context)
      {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (token != null)
          ValidateToken(context, token);

        await _next(context);
      }

      private async void ValidateToken(HttpContext context, string token)
      {
        try
        {
          var tokenHandler = new JwtSecurityTokenHandler();
          var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
          tokenHandler.ValidateToken(token, new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ClockSkew = TimeSpan.Zero
          }, out SecurityToken validatedToken);

          var jwtToken = (JwtSecurityToken)validatedToken;
          var accountId = jwtToken.Claims.First(x => x.Type == "id").Value;

          context.Items["User"] = await _userRepository.GetUserDetails();
        }
        catch
        {

        }
      }
    }
  


    public static class JWTMiddlewareExtensions
    {
      public static IApplicationBuilder UseMiddlewareClassTemplate(this IApplicationBuilder builder)
      {
        return builder.UseMiddleware<JWTMiddleware>();
      }
    }
  }



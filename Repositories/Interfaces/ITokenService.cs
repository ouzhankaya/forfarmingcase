using System;
using forfarmingcase.ViewModels;

namespace forfarmingcase.Repositories.Interfaces
{
  public interface ITokenService
  {
    string BuildToken(string key, string issuer, string audience, LoginVM loginVM);
  }
}

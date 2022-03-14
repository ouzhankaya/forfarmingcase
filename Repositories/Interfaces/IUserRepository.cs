using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using forfarmingcase.Entities;
using forfarmingcase.ViewModels;

namespace forfarmingcase.Repositories.Interfaces
{
  public interface IUserRepository
  {
    Task<List<User>> GetAll();

    Task<User> GetById(int id);

    Task Create(User user);

    Task<bool> Update(User user);

    Task<bool> Delete(int id);
    Task<bool> Login(LoginVM model);
    Task<LoginVM> GetUserDetails();
  }
}

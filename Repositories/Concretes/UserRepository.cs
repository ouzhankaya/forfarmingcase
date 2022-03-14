using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using forfarmingcase.Data;
using forfarmingcase.Entities;
using forfarmingcase.Repositories.Interfaces;
using forfarmingcase.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace forfarmingcase.Repositories.Concretes
{
  public class UserRepository: IUserRepository
  {
    private readonly UserDbContext _context;
    public UserRepository(UserDbContext context)
    {
      _context = context;
    }

    public async Task<bool> Delete(int id)
    {
      var user = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
      _context.Users.Remove(user);
      var status =  await _context.SaveChangesAsync();
      return status > 0;
    }

    public async Task<List<User>> GetAll()
    {
      return await _context.Users.ToListAsync();
    }

    public async Task<User> GetById(int id)
    {
      var user = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
      return user;
    }

    public async Task Create(User user)
    {
      await _context.Users.AddAsync(user);
      await _context.SaveChangesAsync();
    }

    public async Task<bool> Update(User user)
    {
      var existingUser = await GetById(user.Id);
      existingUser.FirstName = user.FirstName;
      existingUser.LastName = user.LastName;
      existingUser.BirthDate = user.BirthDate;
      existingUser.Email = user.Email;
      _context.Users.Update(existingUser);
      var status = await _context.SaveChangesAsync();
      return status > 0;
    }

    public async Task<bool> Login(LoginVM model)
    {
      if(model.Username == "forfarmingcase" && model.Password == "123456")
      {
        return true;
      }

      return false;
    }

    public async Task<LoginVM> GetUserDetails()
    {
      return new LoginVM()
      {
        Username = "forfarmingcase",
      };
    }
  }
}

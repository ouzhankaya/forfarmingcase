using System;
using forfarmingcase.Entities;
using Microsoft.EntityFrameworkCore;

namespace forfarmingcase.Data
{
  public class UserDbContext: DbContext
  {
    public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
    {

    }
      public DbSet<User> Users { get; set; }
  }
}

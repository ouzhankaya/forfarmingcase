using System;
using AutoMapper;
using forfarmingcase.Entities;
using forfarmingcase.ViewModels;

namespace forfarmingcase.Mapping.AutoMapper
{
  public class MappingProfile: Profile
  {
    public MappingProfile()
    {
      CreateMap<User, UserResponseVM>().ReverseMap();
      CreateMap<User, UserUpdateVM>().ReverseMap();
      CreateMap<User, UserCreateVM>().ReverseMap();
    }
  }
}

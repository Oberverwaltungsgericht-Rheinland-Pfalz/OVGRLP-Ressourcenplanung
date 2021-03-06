using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rema.Infrastructure.Models;
using Rema.WebApi.ViewModels;

namespace Rema.WebApi.MappingProfiles
{
  public class UserMappingProfile : Profile
  {
    public UserMappingProfile()
    {
      CreateMap<User, UserViewModel>();

      CreateMap<UserViewModel, User>();
      CreateMap<User, ContactUser>()
          .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name));
    }
  }
}

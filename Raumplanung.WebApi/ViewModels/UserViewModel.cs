using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Raumplanung.Infrastructure.Models;

namespace Raumplanung.WebApi.ViewModels
{
  public class UserViewModel : User
  {
    public List<Role> Roles { get; set; } = new List<Role>();
    public IList<long> SupportGroupIds { get; set; }
  }

  public class UserVMProfile : Profile
  {
    public UserVMProfile()
    {
      CreateMap<User, UserViewModel>()
          .ForMember(dest => dest.SupportGroupIds, opt => opt.MapFrom(src => src.SupplierGroups.Select(e => e.Id)));

      CreateMap<UserViewModel, User>();
      CreateMap<User, ContactUser>()
          .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Name));
    }
  }

  public class Role
  {
    [JsonIgnore]
    public string AdDescription { get; set; }

    public string Name { get; set; }
    public int Level { get; set; }

    public bool HasRole(Role role)
    {
      return role.Level >= Level;
    }
  }

  public class ContactUser
  {
    public long Id { get; set; }
    public string Title { get; set; }
    public string Email { get; set; }
    public string Organisation { get; set; }
  }
}
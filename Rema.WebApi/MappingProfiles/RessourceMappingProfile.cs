using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rema.Infrastructure.Models;
using Rema.WebApi.ViewModels;

namespace Rema.WebApi.MappingProfiles
{
  public class RessourceMappingProfile : Profile
  {
    public RessourceMappingProfile()
    {
      CreateMap<Ressource, RessourceViewModel>();
      CreateMap<RessourceViewModel, Ressource>();
    }
  }
}

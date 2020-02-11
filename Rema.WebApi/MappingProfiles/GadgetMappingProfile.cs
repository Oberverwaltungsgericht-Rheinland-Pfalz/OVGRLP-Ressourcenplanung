using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rema.Infrastructure.Models;
using Rema.WebApi.ViewModels;

namespace Rema.WebApi.MappingProfiles
{
  public class GadgetMappingProfile : Profile
  {
    public GadgetMappingProfile()
    {
      CreateMap<Gadget, GadgetViewModel>()
          .ForMember(dest => dest.SuppliedBy, opt => opt.MapFrom(src => src.SuppliedBy.Id));
      // todo: reverse mapping
    }
  }
}

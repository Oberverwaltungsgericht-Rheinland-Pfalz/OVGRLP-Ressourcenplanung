using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Rema.Infrastructure.Models;
using Rema.WebApi.ViewModels;

namespace Rema.WebApi.MappingProfiles
{
  public class AllocationMappingProfile : Profile
  {
    public AllocationMappingProfile()
    {
      CreateMap<Allocation, AllocationViewModel>()
        .ForMember(dest => dest.RessourceId, opt => opt.MapFrom(src => src.Ressource.Id))
        .ForMember(dest => dest.GadgetsIds, opt => opt.MapFrom(src => src.AllocationGadgets.Select(a => a.GadgetId)));

      CreateMap<AllocationViewModel, Allocation>();
    }
  }
}

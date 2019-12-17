using AutoMapper;
using Raumplanung.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raumplanung.WebApp.ViewModels
{
  public class AllocationPurposeViewModel
  {
    public long Id { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }

    public string Notes { get; set; }
    public string ContactPhone { get; set; }
    public IEnumerable<long> AllocationIds { get; set; }
    public IEnumerable<long> GadgetIds { get; set; }
  }

  public class AllocationPurposeVMProfile : Profile
  {
    public AllocationPurposeVMProfile()
    {
      CreateMap<AllocationPurpose, AllocationPurposeViewModel>()
          .ForMember(dest => dest.GadgetIds, opt => opt.MapFrom(src => src.Gadgets.Select(e => e.GadgetId)))
          .ForMember(dest => dest.AllocationIds, opt => opt.MapFrom(src => src.Allocations.Select(e => e.Id)));

      CreateMap<AllocationPurposeViewModel, AllocationPurpose>();
    }
  }
}
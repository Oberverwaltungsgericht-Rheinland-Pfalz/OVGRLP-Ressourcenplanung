﻿using AutoMapper;
using Raumplanung.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raumplanung.WebApi.ViewModels
{
  public class GadgetViewModel
  {
    public long Id { get; set; }
    public string Title { get; set; }
    public long SuppliedBy { get; set; }
  }

  public class GadgetVMProfile : Profile
  {
    public GadgetVMProfile()
    {
      CreateMap<Gadget, GadgetViewModel>()
          .ForMember(dest => dest.SuppliedBy, opt => opt.MapFrom(src => src.SuppliedBy.Id));
      // todo: reverse mapping
    }
  }
}
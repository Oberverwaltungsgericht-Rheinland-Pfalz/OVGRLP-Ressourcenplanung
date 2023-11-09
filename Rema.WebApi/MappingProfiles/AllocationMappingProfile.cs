// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
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
        .ForMember(dest => dest.RessourceIds, opt => opt.MapFrom(src => src.Ressources.Select(r => r.Id)))
        .ForMember(dest => dest.ScheduleSeries, opt => opt.MapFrom(src => src.ScheduleSeriesGuid))
        .ForMember(dest => dest.HintsForSuppliers, opt => opt.MapFrom(src =>
          src.HintsForSuppliers.Select(e =>
            new SimpleSupplierHint() { GroupId = e.Group.Id, Message = e.Message }
          )))
        .ForMember(dest => dest.GadgetsIds, opt => opt.MapFrom(src => src.Gadgets.Select(a => a.Id)));

      CreateMap<AllocationViewModel, Allocation>()
        .ForMember(dest => dest.HintsForSuppliers, act => act.Ignore());
    }
  }
}

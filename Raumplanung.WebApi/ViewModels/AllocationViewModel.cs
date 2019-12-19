using AutoMapper;
using Raumplanung.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Raumplanung.WebApi.ViewModels
{
  public class AllocationViewModel
  {
    public long Id { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public Boolean IsAllDay { get; set; }

    public MeetingStatus Status { get; set; }

    public long Ressource_id { get; set; }

    public long Purpose_id { get; set; }

    public long CreatedBy_id { get; set; }
    public DateTime CreatedAt { get; set; }

    public DateTime LastModified { get; set; }
    public long LastModifiedBy_id { get; set; }

    public long ApprovedBy_id { get; set; }
    public DateTime ApprovedAt { get; set; }

    public long ReferencePerson_id { get; set; }
  }

  public class AllocationVMProfile : Profile
  {
    public AllocationVMProfile()
    {
      CreateMap<Allocation, AllocationViewModel>()
          .ForMember(dest => dest.Ressource_id, opt => opt.MapFrom(src => src.Ressource.Id))
          .ForMember(dest => dest.Purpose_id, opt => opt.MapFrom(src => src.Purpose.Id)
          );
      CreateMap<AllocationViewModel, Allocation>();
    }
  }

  public class AllocationFilter
  {
    public long UserId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
  }

  public class AllocationRequestEdition
  {
    public long Id { get; set; }
    public int status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
  }
}
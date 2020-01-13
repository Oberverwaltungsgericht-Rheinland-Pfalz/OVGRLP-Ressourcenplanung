using AutoMapper;
using Rema.Infrastructure.Models;

namespace Rema.WebApi.ViewModels
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

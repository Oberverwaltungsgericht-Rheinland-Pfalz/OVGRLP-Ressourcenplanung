using AutoMapper;
using Rema.Infrastructure.Models;

namespace Rema.WebApi.ViewModels
{
  public class GadgetViewModel
  {
    public long Id { get; set; }
    public string Title { get; set; }
    public long SuppliedBy { get; set; }
    public bool IsDeactivated { get; set; }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rema.WebApi.ViewModels
{
  public class MultipleAllocationsViewModel : AllocationViewModel
  {
    public List<string> Dates { get; set; }
  }
}

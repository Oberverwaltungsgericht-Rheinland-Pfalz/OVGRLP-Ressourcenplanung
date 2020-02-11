using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rema.WebApi.ViewModels
{ 
  public class AllocationFilter
  {
    public long UserId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
  }
}

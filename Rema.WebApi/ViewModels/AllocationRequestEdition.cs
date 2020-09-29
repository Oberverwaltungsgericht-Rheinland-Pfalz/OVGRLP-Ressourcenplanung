using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rema.WebApi.ViewModels
{
  public class AllocationRequestEdition
  {
    public long Id { get; set; }
    public int status { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public Boolean IsAllDay { get; set; }
  }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Rema.Infrastructure.Models
{
  [Table("AllocationGagdet")]
  public class AllocationGagdet
  {
    public long AllocationId { get; set; }
    
    public Allocation Allocation { get; set; }

    public long GadgetId { get; set; }

    public Gadget Gadget { get; set; }
  }
}

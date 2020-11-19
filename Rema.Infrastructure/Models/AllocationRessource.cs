using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Rema.Infrastructure.Models
{
  [Table("AllocationRessource")]
  public class AllocationRessource
  {
    public long AllocationId { get; set; }
    
    public Allocation Allocation { get; set; }

    public long RessourceId { get; set; }

    public Ressource Ressource { get; set; }
  }
}

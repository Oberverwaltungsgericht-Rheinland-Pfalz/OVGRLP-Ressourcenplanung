using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rema.Infrastructure.Models
{
  [Obsolete]
  public class AllocationPurpose
  {
    public AllocationPurpose()
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string Title { get; set; }

    [MaxLength(3000)]
    public string Description { get; set; }

    [MaxLength(3000)]
    public string Notes { get; set; }

    public string ContactPhone { get; set; }
    public virtual ICollection<Allocation> Allocations { get; set; }
    public virtual ICollection<GadgetPurpose> Gadgets { get; set; } = new List<GadgetPurpose>();
  }
}

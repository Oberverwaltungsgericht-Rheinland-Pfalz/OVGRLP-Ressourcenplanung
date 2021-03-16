using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rema.Infrastructure.Models
{
  public class Gadget
  {
    public Gadget()
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string Title { get; set; }

    public virtual SupplierGroup SuppliedBy { get; set; }
    public virtual ICollection<Allocation> Allocations { get; set; }
    public bool IsDeactivated { get; set; }
  }
}

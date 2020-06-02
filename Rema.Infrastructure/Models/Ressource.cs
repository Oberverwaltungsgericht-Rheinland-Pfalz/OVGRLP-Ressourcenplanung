using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rema.Infrastructure.Models
{
  public class Ressource
  {
    public Ressource()
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; } // todo: should be unique

    public string FunctionDescription { get; set; }
    
    // todo: delete
    public string Usability { get; set; }
    // todo: delete
    public virtual ICollection<Gadget> Gadgets { get; set; }

    public string SpecialsDescription { get; set; }
    public string Type { get; set; }
  }
}

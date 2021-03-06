using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rema.Infrastructure.Models
{
  public class SupplierGroup
  {
    public SupplierGroup()
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string GroupEmail { get; set; }
    public bool Remind { get; set; }
  }
}

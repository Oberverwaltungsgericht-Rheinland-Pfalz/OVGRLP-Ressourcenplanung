using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Raumplanung.Infrastructure.Models
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
  }
}

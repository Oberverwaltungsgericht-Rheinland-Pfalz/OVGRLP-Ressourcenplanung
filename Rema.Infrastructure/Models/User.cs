using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rema.Infrastructure.Models
{
  public class User
  {
    public User()
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string ActiveDirectoryID { get; set; }

    [Required]
    public string Name { get; set; }

    public string Organisation { get; set; }

    [Required]
    public string Email { get; set; }

    public virtual ICollection<SupplierGroup> SupplierGroups { get; set; }
  }
}

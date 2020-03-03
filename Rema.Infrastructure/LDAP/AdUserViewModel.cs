using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rema.Infrastructure.LDAP
{
  public class AdUserViewModel
  {
    public string ActiveDirectoryID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
  }
}

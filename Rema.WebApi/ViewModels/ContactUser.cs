using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rema.WebApi.ViewModels
{
  public class ContactUser
  {
    public long Id { get; set; }
    public string Title { get; set; }
    public string Email { get; set; }
    public string Organisation { get; set; }
  }
}

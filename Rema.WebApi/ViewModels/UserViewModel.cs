using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using Rema.Infrastructure.Models;

namespace Rema.WebApi.ViewModels
{
  public class UserViewModel : User
  {
    public List<Role> Roles { get; set; } = new List<Role>();
    public IList<long> SupportGroupIds { get; set; }
  }
}

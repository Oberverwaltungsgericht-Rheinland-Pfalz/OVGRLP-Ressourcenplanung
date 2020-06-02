using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rema.WebApi.ViewModels
{
  public class RessourceViewModel
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public string FunctionDescription { get; set; }
    public string SpecialsDescription { get; set; }
    public string Type { get; set; }
  }
}

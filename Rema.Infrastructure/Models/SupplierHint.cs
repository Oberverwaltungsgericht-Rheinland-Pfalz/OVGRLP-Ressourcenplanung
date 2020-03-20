using System;
using System.Collections.Generic;
using System.Text;

namespace Rema.Infrastructure.Models
{
  // keine Datenbanktabelle
  public class SupplierHint
  {
    public SupplierGroup Group { get; set; }
    public string Message { get; set; }
  }
}

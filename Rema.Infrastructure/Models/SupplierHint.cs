// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
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

// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rema.WebApi.ViewModels
{ 
  public class AllocationFilter
  {
    public long UserId { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
  }
}

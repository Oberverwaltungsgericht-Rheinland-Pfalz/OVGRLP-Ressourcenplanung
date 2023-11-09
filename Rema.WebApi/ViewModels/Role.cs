// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Rema.WebApi.ViewModels
{
  public class Role
  {
    [JsonIgnore]
    public string AdDescription { get; set; }

    public string Name { get; set; }

    public int Level { get; set; }

    public bool HasRole(Role role)
    {
      return Level >= role.Level;
    }
  }
}

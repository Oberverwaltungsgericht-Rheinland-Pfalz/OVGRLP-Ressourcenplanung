// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using Microsoft.AspNetCore.Mvc;

namespace Rema.WebApi.Filter
{
  public class AuthorizeAdAttribute : TypeFilterAttribute
  {
    public AuthorizeAdAttribute(string permission)
        : base(typeof(AuthorizeAdActionFilter))
    {
      Arguments = new object[] { permission };
    }
  }
}

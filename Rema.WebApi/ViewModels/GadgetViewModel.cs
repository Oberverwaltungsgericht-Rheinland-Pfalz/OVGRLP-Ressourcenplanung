// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using AutoMapper;
using Rema.Infrastructure.Models;

namespace Rema.WebApi.ViewModels
{
  public class GadgetViewModel
  {
    public long Id { get; set; }
    public string Title { get; set; }
    public long SuppliedBy { get; set; }
    public bool IsDeactivated { get; set; }
  }
}

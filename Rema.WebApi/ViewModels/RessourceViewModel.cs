// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
namespace Rema.WebApi.ViewModels
{
  public class RessourceViewModel
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public string FunctionDescription { get; set; }
    public string SpecialsDescription { get; set; }
    public string Type { get; set; }
    public bool IsDeactivated { get; set; }
  }
}

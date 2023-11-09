// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rema.Infrastructure.Models
{
  public class Ressource
  {
    public Ressource()
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string Name { get; set; } // todo: should be unique

    public string FunctionDescription { get; set; }

    public string SpecialsDescription { get; set; }

    public string Type { get; set; }

    public virtual ICollection<Allocation> Allocations { get; set; }
    public bool IsDeactivated { get; set; }
  }
}

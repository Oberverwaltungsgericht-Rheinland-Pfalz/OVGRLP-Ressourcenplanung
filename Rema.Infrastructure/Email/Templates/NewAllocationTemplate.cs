// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Email.Templates
{
  public class NewAllocationTemplate : EmailTemplate
  {
    public NewAllocationTemplate(Allocation allocation, string bookingType) : base(allocation)
    {
      base.Type = "erstellt";
      _bookingType = bookingType;
    }
    public NewAllocationTemplate(IList<Allocation> allocations, string bookingType) : base(allocations)
    {
      base.Type = "wurde als Serientermin";
      _bookingType = bookingType;
    }

    private string singleId => this.ForSerial ? "" : $"#{this._allocation.Id}";
    private string _multipleIds => !ForSerial? "" : "Termin IDs: #" + string.Join(" #", this._allocations.Select(e => e.Id));
    private string _bookingType;

  public override string Subject => $"Termin {_bookingType}{this.singleId} wurde erstellt";

    public override string ToString()
    {
      return $@"Folgender Eintrag wurde in der Ressourcenplanung {this.Type} {_bookingType}.
Von: {LastModifier}

Raum:
{RessourceName} 

Titel:
{Title}

Reservierung:
{ReserveTime}

Ansprechpartner:
{ReferencePerson}

Telefonnummer:
{ContactPhone}
{GroupsGadgets}

Notizen:
{Notes}
{HintsForSuppliersText}
{_multipleIds}";
    }
  }
}

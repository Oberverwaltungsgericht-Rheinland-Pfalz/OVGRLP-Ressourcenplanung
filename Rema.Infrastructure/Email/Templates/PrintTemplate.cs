// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Text;
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Email.Templates
{
  public class PrintTemplate : EmailTemplate
  {
    public PrintTemplate(Allocation allocation) : base(allocation)
    {
    }
    private string _newStatus { get; set; }

    public override string Subject => "";

    public override string ToString()
    {
      return $@"Termineintrag #{this._allocation.Id} in der Ressourcenplanung.

Erstellt durch: {this._allocation.CreatedBy?.Name}
Zuletzt geändert durch: {LastModifier}
Zuletzt geändert am: {this._allocation.LastModified.ToString("dddd, dd MMMM yyyy")}

Ansprechpartner:
{ReferencePerson}

Raum: {RessourceName} 

Titel:
{Title}

Status:
{Status}

Reservierung:
{ReserveTime}

Telefonnummer:
{ContactPhone}

Hilfsmittel:{GroupsGadgets}

Notizen:
{Notes}
{HintsForSuppliersText}
";
    }
  }
}

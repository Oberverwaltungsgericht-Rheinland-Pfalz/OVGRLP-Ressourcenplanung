// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Text;
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Email.Templates
{
  public class StatusChangedTemplate : EmailTemplate
  {
    public StatusChangedTemplate (Allocation allocation, string newStatus): base(allocation)
    {
      this._newStatus = newStatus;
      base.Type = "geändert";
    }
    private string _newStatus { get; set; }

    public override string Subject => $"Termin #{this._allocation.Id} wurde {_newStatus}";

    public override string ToString()
    {
      return $@"Folgender Eintrag wurde in der Ressourcenplanung {this.Type}.
Von: {LastModifier}

Raum:
{RessourceName} 

Titel:
{Title}

Ihre Buchungsanfrage wurde {_newStatus}

Status:
{Status}

Reservierung:
{ReserveTime}

Ansprechpartner:
{ReferencePerson}

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

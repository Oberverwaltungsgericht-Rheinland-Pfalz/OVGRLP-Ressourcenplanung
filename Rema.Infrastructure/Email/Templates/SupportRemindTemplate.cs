// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Email.Templates
{
  public class SupportRemindTemplate : EmailTemplate
  {
    public SupportRemindTemplate(Allocation allocation) : base(allocation)
    {
    }
    private string _newStatus { get; set; }

    public override string Subject => $"Hifsmittel benötigt für Termin {this._allocation.Title} um {this._allocation.From.ToString("HH:mm")}";

    public override string ToString()
    {
      return $@"Für den Termin #{this._allocation.Id} am  {this._allocation.From} ist die Bereitstellung von Hilfsmitteln durch Sie bzw. eine Nachricht an Sie eingetragen.

Hilfsmittel:{GroupsGadgets}

{HintsForSuppliersText}

Notizen:
{Notes}

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

Erstellt durch: {this._allocation.CreatedBy?.Name}
Zuletzt geändert durch: {LastModifier}
Zuletzt geändert am: {this._allocation.LastModified.ToString("dddd, dd MMMM yyyy")}
";
    }
  }
}

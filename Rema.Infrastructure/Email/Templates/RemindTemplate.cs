// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Email.Templates
{
  public class RemindTemplate : EmailTemplate
  {
    public RemindTemplate(Allocation allocation) : base(allocation)
    {
    }
    private string _newStatus { get; set; }

    public override string Subject => $"Terminerinnerung {this._allocation.Title} am {this._allocation.From}";

    public override string ToString()
    {
      return $@"Möchten Sie Ihren Termin #{this._allocation.Id} am  {this._allocation.From}  in der Ressourcenplanung
wie geplant wahrnehmen?

Wenn dies der Fall ist müssen sie nichts tun. 
Falls Sie ihren Termin nicht wahrnehmen möchten 
setzen sie sich bitte mit Ihrer zuständigen Verwaltungsstelle in Verbindung.

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

Erstellt durch: {this._allocation.CreatedBy?.Name}
Zuletzt geändert durch: {LastModifier}
Zuletzt geändert am: {this._allocation.LastModified.ToString("dddd, dd MMMM yyyy")}
";
    }
  }
}

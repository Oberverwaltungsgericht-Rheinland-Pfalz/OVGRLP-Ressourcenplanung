using System;
using System.Collections.Generic;
using System.Text;
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Email.Templates
{
  public class PrintTemplate : EmailTemplate
  {
    public PrintTemplate (Allocation allocation) : base(allocation)
    {
    }
    private string _newStatus { get; set; }

    public override string Subject => "";

    public override string ToString()
    {
      return $@"Termineintrag in der Ressourcenplanung.

Erstellt durch: {this._allocation.CreatedBy.Name}
Zuletzt geändert durch: {LastModifier}
Zuletzt geändert am: {this._allocation.LastModified.ToString("dddd, dd MMMM y")}

Ansprechpartner:
{ReferencePerson}

Raum: {RessourceName} 

Titel:
{Title}

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

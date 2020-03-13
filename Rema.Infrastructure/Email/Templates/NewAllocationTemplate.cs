using System;
using System.Collections.Generic;
using System.Text;
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Email.Templates
{
  public class NewAllocationTemplate : EmailTemplate
  {
    public NewAllocationTemplate(Allocation allocation, string bookingType) :base(allocation)
    {
      base.Type = "erstellt";
      _bookingType = bookingType;
    }
    public NewAllocationTemplate(IList<Allocation> allocations, string bookingType) : base(allocations)
    {
      base.Type = "wurde als Serientermin erstellt";
      _bookingType = bookingType;
    }

    private string _bookingType;
    public override string Subject => $"[Ressourcenplanungssystem] Termin {_bookingType} wurde erstellt";

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
";
    }
  }
}

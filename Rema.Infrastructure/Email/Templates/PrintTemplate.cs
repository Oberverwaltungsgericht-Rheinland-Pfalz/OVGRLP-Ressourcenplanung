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

Hilfsmittel:{GroupsGadgets}

Notizen:
{Notes}
{HintsForSuppliersText}
";
    }
  }
}

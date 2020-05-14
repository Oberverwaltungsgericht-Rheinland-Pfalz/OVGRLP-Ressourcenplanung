using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Email.Templates
{
  public class DeletedAllocationTemplate: EmailTemplate
  {
    public DeletedAllocationTemplate(Allocation allocation) : base(allocation)
    {
      base.Type = "gelöscht";
    }
    public override string Subject => "Termin wurde gelöscht";

    private Dictionary<SupplierGroup, string> GetGadgetGroups()
    {
      var dict = new Dictionary<SupplierGroup, string>();
      foreach (var allocationGadget in _allocation.AllocationGadgets)
      {
        var group = allocationGadget.Gadget.SuppliedBy;
        var title = allocationGadget.Gadget.Title;
        if (dict.TryGetValue(group, out string oldTitles))
          dict[group] = oldTitles + $", {title}";
        else
          dict.Add(group, title);
      }
      return dict;
    }

    public string DeletedGroupGadgets
    {
      get
      {
        {
          var dict = new Dictionary<SupplierGroup, string>();
          foreach (var allocationGadget in _allocation.AllocationGadgets)
          {
            var group = allocationGadget.Gadget.SuppliedBy;
            var title = allocationGadget.Gadget.Title;
            if (dict.TryGetValue(group, out string oldTitles))
              dict[group] = oldTitles + $", {title}";
            else
              dict.Add(group, title);
          }

          string rValue = $"{System.Environment.NewLine}";
          // Absage an Hilfsmittel Unterstützergruppen
          foreach (var group in dict)
          {
            rValue += $"{System.Environment.NewLine}Die Unterstützungsgruppe {group.Key.Title} wird hiermit darüber benachrichtigt, dass die Bereitstellung der Hilfsmittel {group.Value} nicht mehr benötigt wird.";
          }
          return rValue;
        }
      }
    }

  public override string ToString()
    {
      return $@"Folgender Eintrag wurde in der Ressourcenplanung {this.Type}.
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

Hilfsmittel:{DeletedGroupGadgets}

Notizen:
{Notes}
{HintsForSuppliersText}
";
    }
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Email.Templates
{
  public abstract class EmailTemplate
  {
    public EmailTemplate(Allocation allocation)
    {
      this._allocation = allocation;
    }
    public EmailTemplate(IList<Allocation> allocations)
    {
      _forSerial = true;
      _allocations = allocations;
      _allocation = allocations.First();
    }
    public abstract string Subject { get; }
    public virtual IList<string> GetGroupEmails()
    {
      var rList = new HashSet<string>();
      foreach (var allocationGadget in _allocation.AllocationGadgets)
      {
        rList.Add(allocationGadget.Gadget.SuppliedBy.GroupEmail);
      }
      foreach (var hint in _allocation.HintsForSuppliers)
      {
        rList.Add(hint.Group.GroupEmail);
      }

      return rList.ToList();
    }

    public static string CREATED = "erstellt";
    public static string UPDATED = "geändert";
    public static string DELETED = "gelöscht";
    private bool _forSerial = false;

    public string Type { get; set; }
    protected Allocation _allocation { get; set; }
    protected IList<Allocation> _allocations { get; set; }
    public Ressource Ressource => this._allocation.Ressource;
    protected string RessourceName => this.Ressource.Name;
    protected string Title => this._allocation.Title;
    protected string ReferencePerson { get {
      if (this._allocation.ReferencePerson == null) return "";
      var referencePerson =  $@"{this._allocation.ReferencePerson.Organisation} \ {this._allocation.ReferencePerson.Name}";

      return $@"Ansprechpartner:
{referencePerson}";
      }
    }
    
    protected string Notes => this._allocation.Notes;
    protected string ContactPhone => this._allocation.ContactPhone;
    protected string ReserveTime { get {
        if (!this._forSerial) return TimeOneDateSingle(_allocation);
        return TimeOneDateMultiple();
      }
    }
    private string TimeOneDateSingle (Allocation al) {
      if (al.IsAllDay) return $"{al.From.ToString("dddd, dd MMMM y")}  - {al.To.ToString("dddd, dd MMMM y")} Ganztägig";
      return $"{al.From.ToString("dddd, dd MMMM y HH:mm")}  - {al.To.ToString("dddd, dd MMMM y HH:mm")}";
    }
    private string TimeOneDateMultiple ()
    {
      return string.Join(System.Environment.NewLine, _allocations.Select(TimeOneDateSingle));
    }

    protected string LastModifier => this._allocation.LastModifiedBy.Name;

    protected string AllGadgets { get {
        var list = _allocation.AllocationGadgets.Select(e => e.Gadget.Title);
        return string.Join(", ", list);
      }
    }

    protected string GroupsGadgets { get
      {
        var dictGroups = new Dictionary<SupplierGroup, IList<string>>();

        foreach (var gadget in _allocation.AllocationGadgets.Select(e => e.Gadget))
        {
          var group = gadget.SuppliedBy;
          if (dictGroups.TryGetValue(group, out IList<string> oldTitles))
            oldTitles.Add(gadget.Title);
          else
            dictGroups.Add(group, new List<string>() { gadget.Title });
        }
        string rValue = "";
        foreach(var group in dictGroups.ToList())
        {
          rValue += $"{System.Environment.NewLine}{System.Environment.NewLine}Organisation {group.Key.Title}:\n";
          foreach(var gadget in group.Value)
          {
            rValue += $"{gadget}, ";
          }
          rValue = rValue.Remove(rValue.Length - 2, 1);
        }
        return rValue;
      } 
    }

    protected string HintsForSuppliersText
    {
      get
      {
        string rValue = string.Empty;
        if (_allocation.HintsForSuppliers.Any()) rValue = $@"´{Environment.NewLine}Hinweise an Unterstützergruppen:{Environment.NewLine}{Environment.NewLine}";
        foreach (var entry in _allocation.HintsForSuppliers)
        {
          rValue += $@"An Gruppe {entry.Group.Title}: {entry.Message}{Environment.NewLine}";
        }

        return rValue;
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

Hilfsmittel:{GroupsGadgets}

Notizen:
{Notes}
{HintsForSuppliersText}
";
    }
  }
}
    /*
Folgender Eintrag wurde in der Ressourcenplanung erstellt oder geändert.

Raum:
Konferenzraum E127

Titel:
Videokonferenz

Reservierung:
10.03.2020 10:00:00 - 10.03.2020 13:00:00

Ansprechpartner:
GENSTAKO\steffen.breyer

Telefonnummer:
-30732

Organisation Hausverwaltung:
;#Tische / Stühle stellen;#

Notizen:
VK Rechtshilfeersuchen Spanien; Tische und Stühle für Beteiligte frontal zu VK-Sytem stellen;
    */


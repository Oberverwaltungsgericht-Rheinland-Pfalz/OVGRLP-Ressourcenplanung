// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rema.Infrastructure.Models;

namespace Rema.Infrastructure.Email.Templates
{
  public class GadgetUpdateTemplate : EmailTemplate
  {
    public GadgetUpdateTemplate(Allocation allocation, IList<Gadget> createdGadgets, IList<Gadget> droppedGadgets, IList<string> additionalGroups = null) : base(allocation)
    {
      base.Type = "geändert";
      _droppedGadgets = droppedGadgets;
      _createdGadgets = createdGadgets;
      _additionalGroups = additionalGroups;
    }
    private readonly IList<Gadget> _droppedGadgets;
    private readonly IList<Gadget> _createdGadgets;
    private readonly IList<string> _additionalGroups;

    public override string Subject => "Termin wurde geändert #" + this._allocation.Id;
    public override IList<string> GetGroupEmails()
    {
      (var dictDeleted, var dictCreated) = GetGadgetGroups();

      var rList = new HashSet<string>(this._additionalGroups ?? new List<string>());
      foreach(var del in dictDeleted)
      {
        rList.Add(del.Key.GroupEmail);
      }
      foreach(var crea in dictCreated)
      {
        rList.Add(crea.Key.GroupEmail);
      }
      return rList.ToList();
    }
    private (Dictionary<SupplierGroup , string> dictDeleted, Dictionary<SupplierGroup, string> dictCreated) GetGadgetGroups()
    {
      // entfallene Hilfsmittel benachrichtigen
      var dictDeleted = new Dictionary<SupplierGroup, string>();
      var dictCreated = new Dictionary<SupplierGroup, string>();

      foreach (var gadget in _droppedGadgets)
      {
        var group = gadget.SuppliedBy;
        if (dictDeleted.TryGetValue(group, out string oldTitles))
          dictDeleted[group] = $"{oldTitles}, {gadget.Title}";
        else
          dictDeleted.Add(group, $"{System.Environment.NewLine}-Folgende Hilfsmittel werden nicht mehr benötigt: {gadget.Title}");
      }

      // wegen neuer Hilfsmittel benachrichtigen
      foreach (var alGadget in _createdGadgets)
      {
        var group = alGadget.SuppliedBy;
        if (dictCreated.TryGetValue(group, out string oldTitles))
          dictCreated[group] = $"{oldTitles}, {alGadget.Title}";
        else
          dictCreated.Add(group, $"{System.Environment.NewLine}-Folgende Hilfsmittel werden zusätzlich benötigt: {alGadget.Title}");
      }
      return (dictDeleted, dictCreated);
    }

    public string UpdatedGroupGadgets
    {
      get
      {
        (var dictDeleted, var dictCreated) = GetGadgetGroups();
        string rValue = base.GroupsGadgets;

        if(dictDeleted.Any()) rValue = $"{System.Environment.NewLine}Entfernte Hilfsmittel{System.Environment.NewLine}";
        // Absage an Hilfsmittel Unterstützergruppen
        foreach (var group in dictDeleted)
        {
          rValue += $"{System.Environment.NewLine}Die Unterstützungsgruppe {group.Key.Title} wird hiermit benachrichtigt {group.Value}";
        }

        if (dictCreated.Any()) rValue += $"{System.Environment.NewLine}{System.Environment.NewLine}Hinzugefügte Hilfsmittel{System.Environment.NewLine}";
        foreach (var group in dictCreated)
        {
          rValue += $"{System.Environment.NewLine}Die Unterstützungsgruppe {group.Key.Title} wird hiermit benachrichtigt {group.Value}";
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

Hilfsmittel:{UpdatedGroupGadgets}

Notizen:
{Notes}
{HintsForSuppliersText}
";
    }

  }
}

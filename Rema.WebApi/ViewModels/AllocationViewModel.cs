﻿// SPDX-FileCopyrightText: © 2019 Oberverwaltungsgericht Rheinland-Pfalz <poststelle@ovg.jm.rlp.de>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using AutoMapper;
using Newtonsoft.Json;
using Rema.Infrastructure.Models;

namespace Rema.WebApi.ViewModels
{
  public class AllocationViewModel
  {
    public long Id { get; set; }

    public string Title { get; set; }

    public string Notes { get; set; }

    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public Boolean IsAllDay { get; set; }

    public string ContactPhone { get; set; }

    public MeetingStatus Status { get; set; }

    public IEnumerable<long> RessourceIds { get; set; }

    public IEnumerable<long> GadgetsIds { get; set; }

    public long CreatedById { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModified { get; set; }

    public long LastModifiedById { get; set; }

    public long ApprovedById { get; set; }

    public DateTime ApprovedAt { get; set; }

    public string ReferencePersonId { get; set; }

    public string ScheduleSeries { get; set; }
    public IList<SimpleSupplierHint> HintsForSuppliers { get; set; }

    public override string ToString()
    {
      return JsonConvert.SerializeObject(this);
    }
  }
}

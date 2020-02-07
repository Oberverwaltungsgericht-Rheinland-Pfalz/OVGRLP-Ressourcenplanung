using System;
using System.Collections.Generic;
using AutoMapper;
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

    public MeetingStatus Status { get; set; }

    public long Ressource_id { get; set; }

    public IEnumerable<long> GadgetIds { get; set; }

    public long CreatedBy_id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModified { get; set; }

    public long LastModifiedBy_id { get; set; }

    public long ApprovedBy_id { get; set; }

    public DateTime ApprovedAt { get; set; }

    public long ReferencePerson_id { get; set; }
  }
}

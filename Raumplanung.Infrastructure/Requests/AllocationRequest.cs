using System;
using System.Collections.Generic;
using System.Text;

namespace Raumplanung.Infrastructure.Requests
{
  public class AllocationRequest
  {
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public Boolean IsAllDay { get; set; }

    public long RessourceId { get; set; }

    public long PurposeId { get; set; }

    public long CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }

    public long ReferencePersonUserId { get; set; }
  }
}
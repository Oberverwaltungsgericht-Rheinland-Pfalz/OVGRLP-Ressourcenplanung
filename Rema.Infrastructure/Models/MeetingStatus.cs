using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Rema.Infrastructure.Models
{  public enum MeetingStatus
  {
    [EnumMember(Value = "Draft")]
    Draft = -1,

    [EnumMember(Value = "Pending")]
    Pending = 0,

    [EnumMember(Value = "Acknowledged")]
    Approved = 1,

    [EnumMember(Value = "Clarification")]
    Clarification = 2,

    [EnumMember(Value = "MovedAcknowledge")]
    Moved = 3,

    [EnumMember(Value = "Hidden")]
    Hidden,

    [EnumMember(Value = "Archived")]
    Archived,

    [EnumMember(Value = "Deleted")]
    Deleted
  }
}

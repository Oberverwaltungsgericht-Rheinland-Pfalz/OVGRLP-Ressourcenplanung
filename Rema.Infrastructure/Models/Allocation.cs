using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Rema.Infrastructure.Models
{
  public class Allocation
  {
    public Allocation()
    {
    }

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public string Title { get; set; }

    public string ContactName { get; set; }

    public string ContactPhone { get; set; }
    
    [MaxLength(3000)]
    public string Notes { get; set; }

    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public Boolean IsAllDay { get; set; }

    public MeetingStatus Status { get; set; }

    public virtual Ressource Ressource { get; set; }
    
    public Guid? ScheduleSeriesGuid { get; set; }

    [Required]
    public virtual User CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime LastModified { get; set; }

    public User LastModifiedBy { get; set; }

    public virtual User ApprovedBy { get; set; }

    public DateTime ApprovedAt { get; set; }

    public virtual User ReferencePerson { get; set; }

    public virtual ICollection<AllocationGagdet> AllocationGadgets { get; set; }
  }
}

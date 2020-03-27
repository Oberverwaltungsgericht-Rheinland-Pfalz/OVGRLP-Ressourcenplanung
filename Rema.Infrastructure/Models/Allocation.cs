using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using Newtonsoft.Json;

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
    
    [Column]
    protected string SerializedHints { get; set; }
    public static readonly Expression<Func<Allocation, string>> SerializedHintsExpression = p => p.SerializedHints;

    [NotMapped]
    public IList<SupplierHint> HintsForSuppliers { get 
      {
        if (string.IsNullOrEmpty(this.SerializedHints)) 
          return new List<SupplierHint>();

        return JsonConvert.DeserializeObject<IList<SupplierHint>>(this.SerializedHints);
      } set { // input = value
        this.SerializedHints = JsonConvert.SerializeObject((IList<SupplierHint>)value);
      } 
    }
  }
}

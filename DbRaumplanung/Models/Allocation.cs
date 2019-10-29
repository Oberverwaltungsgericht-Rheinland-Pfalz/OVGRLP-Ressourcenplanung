using DbRaumplanung.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace DbRaumplanung.Models
{
    public class Allocation
    {
        public Allocation() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Boolean IsAllDay { get; set; }


        public MeetingStatus Status { get; set; }

        public virtual Ressource Ressource { get; set; }
        
        public virtual AllocationPurpose Purpose { get; set; }

        [Required]
        public virtual User CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime LastModified { get; set; }
        public User LastModifiedBy { get; set; }

        public virtual User ApprovedBy { get; set; }
        public DateTime ApprovedAt { get; set; }

        public virtual User ReferencePerson { get; set; }
    }

    public enum MeetingStatus
    {
        [EnumMember(Value = "Draft")]
        Draft = -1,

        [EnumMember(Value = "Pending")]
        Pending = 0 ,

        [EnumMember(Value = "Acknowledged")]
        Approved = 1,

        [EnumMember(Value = "Clarification")]
        Clarification = 2 ,

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

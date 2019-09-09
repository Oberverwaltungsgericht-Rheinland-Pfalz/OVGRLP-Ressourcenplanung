using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DbRaumplanung.Models
{
    public class Gadget
    {
        public Gadget() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public string Title { get; set; }
        public virtual SupplierGroup SuppliedBy{ get; set; }
        public virtual ICollection<GadgetPurpose> AllocationPurposes { get; set; } = new List<GadgetPurpose>();
    }

    public class GadgetPurpose
    {
        public GadgetPurpose() { }
        public GadgetPurpose(Gadget gadget, AllocationPurpose allocationPurpose) {
            this.Gadget = gadget;
            this.AllocationPurpose = allocationPurpose;
        }

        public long GadgetId { get; set; }
        public Gadget Gadget{ get; set; }

        public long AllocationPurposeId { get; set; }
        public AllocationPurpose AllocationPurpose{ get; set; }
    }
}

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

    }
}

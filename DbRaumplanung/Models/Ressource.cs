using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DbRaumplanung.Models
{
    public class Ressource
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public string Name { get; set; } // todo: should be unique
        public string FunctionDescription { get; set; }
        public string Usability { get; set; }
        public virtual ICollection<Gadget> Gadgets { get; set; }

        public string SpecialsDescription { get; set; }
    }
}

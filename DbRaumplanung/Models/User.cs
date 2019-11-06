using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DbRaumplanung.Models
{
    public class User
    {
        public User() { }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        // public string ActiveDirectoryID { get; set; }  // hinzufügen
        [Required]
        public string Name { get; set; }
        //public string Organisation { get; set; } // hinzufügen
        [Required]
        public string Email { get; set; }
        public string Mobile { get; set; }  // löschen
        public string Phone { get; set; }  // löschen

        public virtual ICollection<SupplierGroup> SupplierGroups { get; set; }
    }
}

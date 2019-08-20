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

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<SupplierGroup> SupplierGroups { get; set; }
    }
}

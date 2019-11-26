using Newtonsoft.Json;
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
        [JsonIgnore]        
        public string ActiveDirectoryID { get; set; }
        [Required]
        public string Name { get; set; }
        public string Organisation { get; set; }
        [Required]
        public string Email { get; set; }

        public virtual ICollection<SupplierGroup> SupplierGroups { get; set; }
    }
}

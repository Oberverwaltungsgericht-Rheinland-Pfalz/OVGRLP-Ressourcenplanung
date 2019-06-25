using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceLayer.Models
{
    public class AllocationPurposeRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public string ContactPhone { get; set; }
        public virtual IList<long> Gadgets { get; set; }
    }
}

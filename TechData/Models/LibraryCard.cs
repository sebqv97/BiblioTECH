using System;
using System.Collections.Generic;

namespace TechData.Models
{
    public class LibraryCard
    {
        public int Id { get; set; }

        public float Fees { get; set; }

        public DateTime Created { get; set; }

        public virtual IEnumerable<Checkout> Checkouts { get; set; }
    }
}

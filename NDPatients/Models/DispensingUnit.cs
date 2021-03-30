using System;
using System.Collections.Generic;

namespace NDPatients.Models
{
    // Class for DispensingUnit table
    public partial class DispensingUnit
    {
        // Constructor
        public DispensingUnit()
        {
            Medication = new HashSet<Medication>();
        }

        // Properties
        public string DispensingCode { get; set; }

        // Relationships to other tables
        public virtual ICollection<Medication> Medication { get; set; }
    }
}

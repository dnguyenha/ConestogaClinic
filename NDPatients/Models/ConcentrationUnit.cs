using System;
using System.Collections.Generic;

namespace NDPatients.Models
{
    // Class for ConcentrationUnit table
    public partial class ConcentrationUnit
    {
        // Constructor
        public ConcentrationUnit()
        {
            Medication = new HashSet<Medication>();
        }

        // Properties
        public string ConcentrationCode { get; set; }

        // Relationships to other tables
        public virtual ICollection<Medication> Medication { get; set; }
    }
}

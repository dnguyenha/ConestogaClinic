using System;
using System.Collections.Generic;

namespace NDPatients.Models
{
    // Class for MedicationType table
    public partial class MedicationType
    {
        // Construtor
        public MedicationType()
        {
            Medication = new HashSet<Medication>();
        }

        // Properties
        public int MedicationTypeId { get; set; }
        public string Name { get; set; }

        // Relationships to other tables
        public virtual ICollection<Medication> Medication { get; set; }
    }
}

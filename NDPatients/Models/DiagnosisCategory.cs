using System;
using System.Collections.Generic;

namespace NDPatients.Models
{
    // Class for DiagnosisCategory table
    public partial class DiagnosisCategory
    {
        // Constructor
        public DiagnosisCategory()
        {
            Diagnosis = new HashSet<Diagnosis>();
        }

        //Properties
        public int Id { get; set; }
        public string Name { get; set; }

        // Relationships to other tables
        public virtual ICollection<Diagnosis> Diagnosis { get; set; }
    }
}

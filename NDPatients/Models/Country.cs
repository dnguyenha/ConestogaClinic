using System;
using System.Collections.Generic;

namespace NDPatients.Models
{
    // Class for Country table
    public partial class Country
    {
        // Constructor
        public Country()
        {
            Province = new HashSet<Province>();
        }

        // Properties
        public string CountryCode { get; set; }
        public string Name { get; set; }
        public string PostalPattern { get; set; }
        public string PhonePattern { get; set; }
        public double FederalSalesTax { get; set; }

        // Relationships to other tables
        public virtual ICollection<Province> Province { get; set; }
    }
}

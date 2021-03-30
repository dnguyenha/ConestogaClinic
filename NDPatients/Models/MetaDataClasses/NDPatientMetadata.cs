using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NDClassLibrary;
using System.Linq;

namespace NDPatients.Models
{
    [ModelMetadataType(typeof(NDPatientMetadata))]
    public partial class Patient : IValidatableObject
    {
        //private readonly PatientsContext context;
        //public Patient(PatientsContext context)
        //{
        //    this.context = context;
        //}

        readonly PatientsContext context = new PatientsContext();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            bool isValid;

            // Validate First Name
            // Check for non-blank
            if (FirstName == null || FirstName.Trim() == "")
                yield return new ValidationResult("First Name cannot be empty or just blanks", new[] { nameof(FirstName) });
            else
            {
                // Capitalize
                FirstName = NDValidations.NDCapitalize(FirstName);
            }

            // Validate Last Name
            // Check for non-blank
            if (LastName == null || LastName.Trim() == "")
                yield return new ValidationResult("Last Name cannot be empty or just blanks", new[] { nameof(LastName) });
            else
            {
                // Capitalize
                LastName = NDValidations.NDCapitalize(LastName);
            }

            // Validate Gender            
            // Check for non-blank
            if (Gender == null || Gender.Trim() == "")
                yield return new ValidationResult("Gender cannot be empty or just blanks", new[] { nameof(Gender) });
            else
            {
                // Capitalize
                Gender = NDValidations.NDCapitalize(Gender);
                if (Gender != "M" && Gender != "F" && Gender != "X")
                    yield return new ValidationResult("Gender must be either 'M', 'F' or 'X'", new[] { nameof(Gender) });
            }

            // Capitalize
            Address = NDValidations.NDCapitalize(Address);
            City = NDValidations.NDCapitalize(City);

            // Validate ProvinceCode
            if (ProvinceCode != null)
            {
                ProvinceCode = ProvinceCode.ToUpper();

                // Check if ProvinceCode exists in DB
                var province = context.Province.Where(p => p.ProvinceCode == ProvinceCode).FirstOrDefault();
                if (province == null)
                    yield return new ValidationResult("Province Code is not on file", new[] { nameof(ProvinceCode) });
            }

            // Validate Postal Code
            if (PostalCode != null)
            {
                if (string.IsNullOrEmpty(ProvinceCode))
                    yield return new ValidationResult("Province Code is required to validate Postal Code", new[] { nameof(ProvinceCode) });
                else
                {
                    // Format Postal Code to 'A3A 3A3'
                    PostalCode = NDValidations.NDPostalCodeFormat(PostalCode);
                    if (PostalCode == string.Empty)
                        yield return new ValidationResult("Postal Code must match pattern: A3A 3A3", new[] { nameof(PostalCode) });

                    isValid = NDValidations.NDIsValidPostalCodeCanada(ProvinceCode.ToUpper().Trim(), PostalCode);
                    if (isValid == false)
                    {
                        yield return new ValidationResult("First letter of Postal Code not valid for given Province", new[] { nameof(PostalCode) });

                        // Validate and format US Zip Code
                        string zipCode = PostalCode;
                        isValid = NDValidations.NDZipCodeValidation(ref zipCode);
                        if (isValid == false)
                            yield return new ValidationResult("Postal Code is invalid", new[] { nameof(PostalCode) });
                        else
                            PostalCode = zipCode;
                    }
                }
            }

            // Validate OHIP
            if (Ohip != null)
            {
                isValid = NDValidations.NDOhipValidation(Ohip);
                if (isValid)
                    Ohip = Ohip.ToUpper().Trim();
                else
                    yield return new ValidationResult("OHIP, if provided, must match pattern: 1234-123-123-XX", new[] { nameof(Ohip) });
            }

            // Validate Home Phone
            string phone = HomePhone;
            isValid = NDValidations.NDPhoneValidation(ref phone);
            if (!isValid)
                yield return new ValidationResult("Home Phone, if provided, must be 10 digits: 123-123-1234", new[] { nameof(HomePhone) });
            else
                HomePhone = phone;

            // Validate Date Of Birth not in the future
            if (DateOfBirth != null && DateOfBirth > DateTime.Now)
                yield return new ValidationResult("Date Of Birth cannot be in the future", new[] { nameof(DateOfBirth) });

            // Validate Date Of Deceased
            if (Deceased == false)
            {
                if (DateOfDeath != null)
                    yield return new ValidationResult("Deceased must be true if Date of Death is provided", new[] { nameof(Deceased) });
            }
            else
            {
                if (DateOfDeath == null)
                    yield return new ValidationResult("If Deceased is true, a Date Of Death is required", new[] { nameof(DateOfDeath) });
                else if (DateOfDeath > DateTime.Now)
                    yield return new ValidationResult("Date Of Death cannot be in the future", new[] { nameof(DateOfDeath) });
                else if (DateOfDeath < DateOfBirth)
                    yield return new ValidationResult("Date Of Death cannot be before Date Of Birth", new[] { nameof(DateOfDeath) });
            }
        }
    }

    // Patient partial class to add FullName attribute
    public partial class Patient
    {
        [Display(Name = "Patient Name")]
        public string FullName { get { return LastName + ", " + FirstName; } }
    }

    public class NDPatientMetadata
    {
        public int PatientId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Street Address")]
        public string Address { get; set; }

        public string City { get; set; }

        [Display(Name = "Province Code")]
        public string ProvinceCode { get; set; }

        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Display(Name = "OHIP")]
        public string Ohip { get; set; }

        [Display(Name = "Date Of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DateOfBirth { get; set; }

        public bool Deceased { get; set; }

        [Display(Name = "Date Of Death")]
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:dd MMM yyyy}")]
        public DateTime? DateOfDeath { get; set; }

        [Display(Name = "Home Phone")]
        public string HomePhone { get; set; }

        public string Gender { get; set; }

        public virtual Province ProvinceCodeNavigation { get; set; }
        public virtual ICollection<PatientDiagnosis> PatientDiagnosis { get; set; }
    }
}

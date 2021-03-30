using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace NDClassLibrary
{
    public static class NDValidations
    {
        // Capitalize first letter of each word
        public static string NDCapitalize(string strInput)
        {
            if (string.IsNullOrEmpty(strInput))
            {
                return string.Empty;
            }

            // Create a TextInfo based on the "en-US" culture.
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

            // Capitalize first letter of each word
            return myTI.ToTitleCase(strInput.ToLower().Trim());
        }

        // Extract all digits in a string (return a new string containing all digits found)
        public static string NDExtractDigits(string strInput)
        {
            if (!string.IsNullOrEmpty(strInput))
            {
                //string strOutput = Regex.Match(strInput.Trim(), @"\d+").Value;
                string strOutput = new string(strInput.Where(Char.IsDigit).ToArray());
                return strOutput;
            }

            return string.Empty;
        }

        // Validate postal code of Canada (ie: A2B 3C4)
        // Accept space, either upper or lower case
        public static bool NDPostalCodeValidation(string strInput)
        {
            if (!string.IsNullOrEmpty(strInput))
            {
                bool foundMatch = Regex.IsMatch(strInput.ToUpper().Trim(), @"\A[ABCEGHJKLMNPRSTVXY]\d[A-Z] ?\d[A-Z]\d\z");
                return foundMatch;
            }
            else // allow postal code to be optional
            {
                return true;
            }
        }

        // Add space in between postal code of Canada (ie: A2B3C4 -> A2B 3C4)
        public static string NDPostalCodeFormat(string strInput)
        {
            bool isValid = NDPostalCodeValidation(strInput);

            if (isValid == false
                || (isValid == true && string.IsNullOrEmpty(strInput)))
            {
                return string.Empty;
            }

            string strOutput = strInput.ToUpper();
            if (strInput.Length == 6)
            {
                // Add space
                strOutput = strInput.Substring(0, 3) + " " + strInput[3..];
            }

            return strOutput.ToUpper();
        }

        // Validate Postal Code is corresponding to Province Code of Canada
        public static bool NDIsValidPostalCodeCanada(string provinceCode, string postalCode)
        {
            if (string.IsNullOrEmpty(provinceCode) || string.IsNullOrEmpty(postalCode))
                return false;

            bool isValid = false;

            switch(provinceCode)
            {
                case "NL":
                    if (postalCode.Substring(0, 1) == "A")
                        isValid = true;
                    break;
                case "NS":
                    if (postalCode.Substring(0, 1) == "B")
                        isValid = true;
                    break;
                case "PE":
                    if (postalCode.Substring(0, 1) == "C")
                        isValid = true;
                    break;
                case "NB":
                    if (postalCode.Substring(0, 1) == "E")
                        isValid = true;
                    break;
                case "QC":
                    if (postalCode.Substring(0, 1) == "G"
                        || postalCode.Substring(0, 1) == "H"
                        || postalCode.Substring(0, 1) == "J")
                        isValid = true;
                    break;
                case "ON":
                    if (postalCode.Substring(0, 1) == "K"
                        || postalCode.Substring(0, 1) == "L"
                        || postalCode.Substring(0, 1) == "M"
                        || postalCode.Substring(0, 1) == "N"
                        || postalCode.Substring(0, 1) == "P")
                        isValid = true;
                        break;
                case "MB":
                    if (postalCode.Substring(0, 1) == "R")
                        isValid = true;
                    break;
                case "SK":
                    if (postalCode.Substring(0, 1) == "S")
                        isValid = true;
                    break;
                case "AB":
                    if (postalCode.Substring(0, 1) == "T")
                        isValid = true;
                    break;
                case "BC":
                    if (postalCode.Substring(0, 1) == "V")
                        isValid = true;
                    break;
                case "NU":
                case "NT":
                    if (postalCode.Substring(0, 1) == "X")
                        isValid = true;
                    break;
                case "YT":
                    if (postalCode.Substring(0, 1) == "Y")
                        isValid = true;
                    break;
                default:
                    break;
            }

            return isValid;
        }

        // Validate and format US Zip Code
        public static bool NDZipCodeValidation(ref string strInput)
        {
            if (string.IsNullOrEmpty(strInput))
            {
                strInput = string.Empty;
                return true;
            }

            string digits = NDExtractDigits(strInput.Trim());

            if (digits.Length == 5)
            {
                strInput = digits;
                return true;
            }
            else if (digits.Length == 9)
            {
                // Format to notation: 12345-1234 
                strInput = strInput.Substring(0, 5) + "-" + strInput[5..];
                return true;
            }
            else
            {
                return false;
            }
        }

        // Validate OHIP to match patern: 1234-123-123-XX
        public static bool NDOhipValidation(string strInput)
        {
            if (!string.IsNullOrEmpty(strInput))
            {
                bool foundMatch = Regex.IsMatch(strInput.ToUpper().Trim(), @"\A\d\d\d\d-?\d\d\d-?\d\d\d-?[A-Z][A-Z]\z");
                return foundMatch;
            }
            else // allow postal code to be optional
            {
                return true;
            }
        }

        // Validate Phone Number: 1234567890
        // Format to dash notation: 123-456-7890
        public static bool NDPhoneValidation(ref string strInput)
        {
            if (string.IsNullOrEmpty(strInput))
            {
                strInput = string.Empty;
                return true;
            }

            // Exact 10 digits
            if (strInput.Length != 10)
                return false;

            bool foundMatch = Regex.IsMatch(strInput.ToUpper().Trim(), @"\A\d\d\d\d\d\d\d\d\d\d\z");
            string phone = strInput;
            if (foundMatch == true)
            {
                //Format to dash notation: 123 - 456 - 7890
                strInput = phone.Substring(0, 3) + "-" + phone.Substring(3, 3) + "-" + phone[6..];
            }

            return foundMatch;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PrintManagement.Domain.Validation
{
    public class ValidateInput
    {
        public static bool IsValidEmail(string email)
        {
            string regex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, regex);
        }
        public static bool IsValidPhoneNumber(string phoneNumber) 
        {
            string regex = @"^(0|84)(3|9)\d{8}$";
            return Regex.IsMatch(phoneNumber, regex);
        }

    }
}

using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace PrintManagement.Application.Payloads.Requests.Users
{
    public class Register
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }    
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "PhoneNumber is required")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "FullName is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Date of Birth is required")]
        public DateTime Dob { get; set; }
        public string Avatar { get; set; }
    }
}

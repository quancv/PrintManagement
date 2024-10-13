using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Respones.Users
{
    public class Login_Response
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}

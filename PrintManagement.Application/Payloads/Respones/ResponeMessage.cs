using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.Payloads.Respones
{
    public class ResponeMessage
    {
        public static string GetEmailSuccessMessage(string email)
        {
            return $"Send to {email} successfully";
        }
    }
}

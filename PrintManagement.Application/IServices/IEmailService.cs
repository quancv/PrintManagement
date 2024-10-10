using PrintManagement.Application.Handle.HandleEmail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.IServices
{
    public interface IEmailService
    {
        string SendMail(EmailMessage emailMessage);
    }
}

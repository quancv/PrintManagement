using PrintManagement.Application.Payloads.Requests.Users;
using PrintManagement.Application.Payloads.Respones;
using PrintManagement.Application.Payloads.Respones.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManagement.Application.IServices
{
    public interface IAuthService
    {
        Task<ResponeObject<UserInfor>> Register(Register register);
        Task<string> ConfirmRegister(string confirmCode);

    }
}

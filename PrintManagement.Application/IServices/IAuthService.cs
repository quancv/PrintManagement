using PrintManagement.Application.Payloads.Requests.Users;
using PrintManagement.Application.Payloads.Respones;
using PrintManagement.Application.Payloads.Respones.Users;
using PrintManagement.Domain.Entities;
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
        Task<ResponeObject<Login_Response>> Login(Login_Request login);
        Task<ResponeObject<Login_Response>> GetJwtAsync(User user);
        Task<ResponeObject<UserInfor>> ChangePassword(int userId, ChangePassword_Request changePassword_Request);
        Task<string> ForgotPassword(string email);
        Task<string> ConfirmAndSetPassword(ForgotPassword_Request forgotPassword_Request);
    }
}

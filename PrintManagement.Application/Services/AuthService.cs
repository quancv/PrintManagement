using AutoMapper;
using Microsoft.AspNetCore.Http;
using PrintManagement.Application.Handle.HandleEmail;
using PrintManagement.Application.IServices;
using PrintManagement.Application.Payloads.Requests.Users;
using PrintManagement.Application.Payloads.Respones;
using PrintManagement.Application.Payloads.Respones.Users;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BCryptNet = BCrypt.Net.BCrypt;

namespace PrintManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IBaseRepository<ConfirmEmail> _baseConfirmRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        public AuthService(IBaseRepository<User> baseUserRepository, IBaseRepository<ConfirmEmail> baseConfirmRepository, IEmailService emailService, IMapper mapper)
        {
            _baseUserRepository = baseUserRepository;
            _baseConfirmRepository = baseConfirmRepository;
            _emailService = emailService;
            _mapper = mapper;
        }

        public async Task<ResponeObject<UserInfor>> Register(Register register)
        {
            var user = _mapper.Map<User>(register);
            user.Password = BCryptNet.HashPassword(register.Password);
            await _baseUserRepository.CreateAsync(user);
            ConfirmEmail confirmEmail = new ConfirmEmail
            {
                UserId = user.Id,
                ConfirmCode = GenerateCode(),
                ExpiryTime = DateTime.Now.AddMinutes(1),
                isConfirm = false
            };
            await _baseConfirmRepository.CreateAsync(confirmEmail);
            var emailMessage = new EmailMessage ( new string[] { user.Email }, "Confirm your account", $"Confirm Code: {confirmEmail.ConfirmCode}" );
            var responeMessage = _emailService.SendMail(emailMessage);
            return new ResponeObject<UserInfor>
            {
                Status = StatusCodes.Status201Created,
                Message = "Register Successfully. Please check your email",
                Data = _mapper.Map<UserInfor>(user)
            };
        }
        public async Task<string> ConfirmRegister(string confirmCode)
        {
            try
            {
                var code = await _baseConfirmRepository.GetAsync(x => x.ConfirmCode.Equals(confirmCode));
                if (code == null)
                    return "Incorrect, Please try again";
                var user = await _baseUserRepository.GetAsync(x => x.Id == code.UserId);
                if (code.ExpiryTime < DateTime.UtcNow)
                    return "Expiry Time";
                user.UserStatus = Domain.Enumerates.ConstantEnums.UserStatusEnum.Activated;
                code.isConfirm = true;
                await _baseConfirmRepository.UpdateAsync(code);
                await _baseUserRepository.UpdateAsync(user);
                return "Verify Success";
            }
            catch
            {
                throw;
            }
        }
        private string GenerateCode()
        {
            Random random = new Random();
            int code = random.Next(100000, 1000000);
            return code.ToString();
        }
    }
}

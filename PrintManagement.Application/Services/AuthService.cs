using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PrintManagement.Application.Handle.HandleEmail;
using PrintManagement.Application.IServices;
using PrintManagement.Application.Payloads.Requests.Users;
using PrintManagement.Application.Payloads.Respones;
using PrintManagement.Application.Payloads.Respones.Users;
using PrintManagement.Domain.Entities;
using PrintManagement.Domain.Enumerates;
using PrintManagement.Domain.IRepositories;
using PrintManagement.Domain.Validation;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BCryptNet = BCrypt.Net.BCrypt;

namespace PrintManagement.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<User> _baseUserRepository;
        private readonly IBaseRepository<ConfirmEmail> _baseConfirmRepository;
        private readonly IBaseRepository<Permissions> _basePermissionsRepository;
        private readonly IBaseRepository<Role> _baseRoleRepository;
        private readonly IBaseRepository<RefreshToken> _baseRefreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AuthService(IBaseRepository<User> baseUserRepository, IBaseRepository<ConfirmEmail> baseConfirmRepository, IBaseRepository<Permissions> basePermissionsRepository, IUserRepository userRepository,
            IBaseRepository<Role> baseRoleRepository, IBaseRepository<RefreshToken> baseRefreshTokenRepository, IEmailService emailService, IMapper mapper, IConfiguration configuration)
        {
            _baseUserRepository = baseUserRepository;
            _baseConfirmRepository = baseConfirmRepository;
            _basePermissionsRepository = basePermissionsRepository;
            _baseRoleRepository = baseRoleRepository;
            _baseRefreshTokenRepository = baseRefreshTokenRepository;
            _userRepository = userRepository;
            _emailService = emailService;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task<ResponeObject<UserInfor>> Register(Register register)
        {
            try
            {
                if (!ValidateInput.IsValidEmail(register.Email))
                {
                    return new ResponeObject<UserInfor>()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid Email",
                        Data = null
                    };
                }
                if (!ValidateInput.IsValidPhoneNumber(register.PhoneNumber))
                {
                    return new ResponeObject<UserInfor>()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Invalid PhoneNumber",
                        Data = null
                    };
                }
                if (await _userRepository.GetUserByEmail(register.Email) != null)
                {
                    return new ResponeObject<UserInfor>()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Email is exist in the system",
                        Data = null
                    };
                }
                if (await _userRepository.GetUserByPhoneNumber(register.PhoneNumber) != null)
                {
                    return new ResponeObject<UserInfor>()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "PhoneNumber is exist in the system",
                        Data = null
                    };
                }
                if (await _userRepository.GetUserByPhoneNumber(register.Username) != null)
                {
                    return new ResponeObject<UserInfor>()
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "UserName is exist in the system",
                        Data = null
                    };
                }
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
                await _userRepository.AddRoleAsync(user, new List<int> { (int)ConstantEnums.Role.Admin });
                var emailMessage = new EmailMessage(new string[] { user.Email }, "Confirm your account", $"Confirm Code: {confirmEmail.ConfirmCode}");
                var responeMessage = _emailService.SendMail(emailMessage);
                return new ResponeObject<UserInfor>
                {
                    Status = StatusCodes.Status201Created,
                    Message = "Register Successfully. Please check your email",
                    Data = _mapper.Map<UserInfor>(user)
                };
            }
            catch (Exception ex)
            {
                return new ResponeObject<UserInfor>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = "Error: " + ex.Message,
                    Data = null
                };
            }
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

        public async Task<ResponeObject<Login_Response>> Login(Login_Request login)
        {
            var user = await _baseUserRepository.GetAsync(x => x.Username.Equals(login.Username));
            if(user == null)
            {
                return new ResponeObject<Login_Response>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "UserName is incorret",
                    Data = null
                };
            }
            if (user.UserStatus is ConstantEnums.UserStatusEnum.UnActivated)
                return new ResponeObject<Login_Response>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "The account is not verify",
                    Data = null
                };
            bool checkPass = BCryptNet.Verify(login.Password, user.Password);
            if (!checkPass)
            {
                return new ResponeObject<Login_Response>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Password is incorret",
                    Data = null
                };
            }
            return new ResponeObject<Login_Response>
            {
                Status = StatusCodes.Status200OK,
                Message = "Login Success",
                Data = new Login_Response
                {
                    AccessToken = GetJwtAsync(user).Result.Data.AccessToken,
                    RefreshToken = GetJwtAsync(user).Result.Data.RefreshToken
                }
            };

        }

        public async Task<ResponeObject<Login_Response>> GetJwtAsync(User user)
        {
            var permissions = await _basePermissionsRepository.GetAllAsync(x => x.UserId == user.Id);
            var roles = await _baseRoleRepository.GetAllAsync();
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("Username", user.Username.ToString()),
                new Claim("Email", user.Email.ToString()),
                new Claim("PhoneNumber", user.PhoneNumber.ToString()),
            };
            foreach (var permission in permissions)
            {
                foreach (var role in roles)
                {
                    if(permission.RoleId == role.Id)
                    {
                        claims.Add(new Claim("Permission", role.RoleName.ToString()));
                    }
                }
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );
            _ = int.TryParse(_configuration["JWT:ExpiryRefreshToken"], out int expiryTimeRefreshToken);
            RefreshToken refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = GenerateRefreshToken(),
                CreateTime = DateTime.UtcNow,
                ExpiryTime = DateTime.UtcNow.AddMinutes(expiryTimeRefreshToken)
            };
            await _baseRefreshTokenRepository.CreateAsync(refreshToken);
            return new ResponeObject<Login_Response>
            {
                Status = StatusCodes.Status201Created,
                Message = "Create Success",
                Data = new Login_Response
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = refreshToken.Token
                }
            };

        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new Byte[64];
            var range = RandomNumberGenerator.Create();
            range.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<ResponeObject<UserInfor>> ChangePassword(int userId, ChangePassword_Request changePassword_Request)
        {
            var user =  await _userRepository.GetUserById(userId);
            if (user == null)
            {
                return new ResponeObject<UserInfor>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Not found",
                    Data = null
                };
            }
            bool checkPass = BCryptNet.Verify(changePassword_Request.OldPassword, user.Password);
            if (!checkPass)
            {
                return new ResponeObject<UserInfor>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Password is incorrect",
                    Data = null
                };
            }
            if (!changePassword_Request.NewPassword.Equals(changePassword_Request.ConfirmPassword))
            {
                return new ResponeObject<UserInfor>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Password is not match",
                    Data = null
                };
            }
            user.Password = BCryptNet.HashPassword(changePassword_Request.NewPassword);
            user.UpdateTime = DateTime.UtcNow;
            await _baseUserRepository.UpdateAsync(user);
            return new ResponeObject<UserInfor>
            {
                Status = StatusCodes.Status200OK,
                Message = "Change Password success",
                Data = _mapper.Map<UserInfor>(user)
            };

        }

        public async Task<string> ForgotPassword(string email)
        {
            try
            {
                var user = await _userRepository.GetUserByEmail(email);
                if (user == null)
                {
                    return "Email does not exist in the system";
                }
                var confirms = await _baseConfirmRepository.GetAllAsync(x => x.UserId == user.Id);
                if (confirms.ToList().Count > 0)
                {
                    foreach (var c in confirms)
                    {
                        await _baseConfirmRepository.DeleteAsync(c.Id);
                    }
                }
                ConfirmEmail confirm = new ConfirmEmail
                {
                    UserId = user.Id,
                    ConfirmCode = GenerateCode(),
                    ExpiryTime = DateTime.Now.AddMinutes(2),
                    isConfirm = false
                };
                await _baseConfirmRepository.CreateAsync(confirm);

                var confirmEmail = new EmailMessage(new string[] { user.Email }, "Reset your password", $"Confirm code: {confirm.ConfirmCode}");
                _emailService.SendMail(confirmEmail);

                return "Please check your email, confirm code has been sent to your email address";
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        public async Task<string> ConfirmAndSetPassword(ForgotPassword_Request forgotPassword_Request)
        {
            try
            {
                if (forgotPassword_Request == null)
                {
                    return "Not Null Here";
                }
                var confirmEmail = await _baseConfirmRepository.GetAsync(x => x.ConfirmCode == forgotPassword_Request.ConfirmCode);
                if (confirmEmail == null)
                {
                    return "Inccorect code";
                }
                if (confirmEmail.ExpiryTime < DateTime.Now)
                {
                    return "Expiry Time";
                }
                if (!forgotPassword_Request.NewPassword.Equals(forgotPassword_Request.ConfirmPassword))
                {
                    return "Password is not match";
                }
                var user = await _userRepository.GetUserById(confirmEmail.UserId);
                user.Password = BCryptNet.HashPassword(forgotPassword_Request.NewPassword);
                user.UpdateTime = DateTime.Now;
                await _baseUserRepository.UpdateAsync(user);
                return "Set Password Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }
    }
}

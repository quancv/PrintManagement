using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.Constants;
using PrintManagement.Application.IServices;
using PrintManagement.Application.Payloads.Requests.Users;
using PrintManagement.Application.Payloads.Respones.Users;
using PrintManagement.Application.Payloads.Respones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace PrintManagement.API.Controllers
{
    [Route(Constant.DefaultRoute.DEFAULT_CONTROLLER_ROUTE)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
           
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Register register)
        {
            return Ok(await _authService.Register(register));
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmAccount(string code)
        {
            return Ok(await _authService.ConfirmRegister(code));
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login_Request login)
        {
            return Ok(await _authService.Login(login)); 
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole(int userId, List<int> roleIds)
        {
            return Ok(await _authService.AddRoleAsync(userId, roleIds));
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword_Request changePassword_Request)
        {
            int userId = int.Parse(HttpContext.User.FindFirst("Id").Value);
            return Ok(await _authService.ChangePassword(userId, changePassword_Request));
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            return Ok(await _authService.ForgotPassword(email));
        }
        [HttpPut]
        public async Task<IActionResult> ConfirmAndSetPassword(ForgotPassword_Request forgotPassword_Request)
        {
            return Ok(await _authService.ConfirmAndSetPassword(forgotPassword_Request));
        }

    }
}

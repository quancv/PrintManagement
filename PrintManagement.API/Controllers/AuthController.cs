using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrintManagement.Application.Constants;
using PrintManagement.Application.IServices;
using PrintManagement.Application.Payloads.Requests.Users;

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
    }
}

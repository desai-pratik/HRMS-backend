using Alpha.Controllers;
using Hrms.Model;
using Hrms.Process;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Hrms.Provider.AccessProvider;

namespace HRMS.Controllers
{
    [ApiController, AllowAnonymous, Route("api/[controller]")]
    public class AuthController : BaseController
    {
        [HttpPost(nameof(Login))] public async Task<IActionResult> Login([FromBody] AuthModel data) => SendResponse(await LoginProcess.Login(data), true);
        [HttpPost(nameof(ForgetPassword) + "/{username}")] public async Task<IActionResult> ForgetPassword([FromRoute] string username) => SendResponse(await LoginProcess.ForgotPassword(username));
    }
}

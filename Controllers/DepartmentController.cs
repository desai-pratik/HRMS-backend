using Alpha.Controllers;
using Hrms.Model;
using Hrms.Process;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Hrms.Provider.AccessProvider;

namespace Hrms.Controllers
{
    [Authorize(Roles = nameof(SystemUserType.Admin)), Route("api/[controller]")]
    public class DepartmentController : BaseController
    {
        [HttpGet(nameof(Get))] public async Task<IActionResult> Get() => SendResponse(await DepartmentProcess.Get(), true);
        [HttpPost(nameof(Save))] public async Task<IActionResult> Save([FromBody] Department data) => SendResponse(await DepartmentProcess.Save(data), true);
    }
}

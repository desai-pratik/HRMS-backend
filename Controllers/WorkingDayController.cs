using Alpha.Controllers;
using Hrms.Model;
using Hrms.Process;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static Hrms.Provider.AccessProvider;

namespace Hrms.Controllers
{
    [Authorize(Roles = nameof(SystemUserType.Admin)), Route("api/[controller]")]
    public class WorkingDayController : BaseController
    {
        [HttpGet(nameof(GetWorkingDay))] public async Task<IActionResult> GetWorkingDay() => SendResponse(await TotalWorkingDayProcess.GetWorkingDay(), false);
        [HttpPost(nameof(AddWorkingDay))] public async Task<IActionResult> AddWorkingDay([FromBody] TotalWorkingDays data) => SendResponse(await TotalWorkingDayProcess.AddWorkingDay(data), true);
    }
}

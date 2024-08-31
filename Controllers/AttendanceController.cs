using Alpha.Controllers;
using Hrms.Model;
using Hrms.Process;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hrms.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : BaseController
    {
        [HttpGet(nameof(GetAttendance))] public async Task<IActionResult> GetAttendance() => SendResponse(await AttendanceProcess.GetAttendance(), true);
        [HttpPost(nameof(AddAttendance))] public async Task<IActionResult> AddAttendance([FromBody] Attendance data) => SendResponse(await AttendanceProcess.AddAttendance(data), true);

    }
}

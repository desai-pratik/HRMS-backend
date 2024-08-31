using Alpha.Controllers;
using Hrms.Model;
using Hrms.Process;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Hrms.Provider.AccessProvider;

namespace Hrms.Controllers
{
    // admin approve
    [Authorize(Roles = nameof(SystemUserType.Admin)), Route("api/[controller]")]
    public class LeaveRequestStatusController : BaseController
    {
        readonly LeaveApplicationProcess process;
        public LeaveRequestStatusController([FromServices] Employee leaveApplication) { process = new() { CurrentEmployee = leaveApplication }; }
        [HttpPost] public async Task<IActionResult> Post([FromBody] LeaveApproval data) => SendResponse(await process.UpdateLeaveRequestStatus(data), true);
    }

    // user send
    [Authorize(Roles = nameof(SystemUserType.User)), Route("api/[controller]")]
    public class LeaveApplicationController : BaseController
    {
        readonly LeaveApplicationProcess process;
        public LeaveApplicationController([FromServices] Employee leaveApplication) { process = new() { CurrentEmployee = leaveApplication }; }
        [HttpPost] public async Task<IActionResult> Post([FromBody] LeaveApplication data) => SendResponse(await process.LeaveRequest(data), true);
    }

    //user and admin display
    [ApiController, Route("api/[controller]")]
    public class LeaveApplicationGetController : BaseController
    {
        readonly LeaveApplicationProcess process;
        public LeaveApplicationGetController([FromServices] Employee leaveApplication) { process = new() { CurrentEmployee = leaveApplication }; }
        [HttpGet] public async Task<IActionResult> Get() => SendResponse(await process.GetUserBy(), true);
    }
}

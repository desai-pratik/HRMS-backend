using Alpha.Controllers;
using Hrms.Model;
using Hrms.Process;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Hrms.Provider.AccessProvider;

namespace Hrms.Controllers
{
    [Authorize(Roles = nameof(SystemUserType.Admin)), Route("api/[controller]")] 
    public class PayRollController : BaseController
    {
        [HttpGet(nameof(PayRollGet))] public async Task<IActionResult> PayRollGet() => SendResponse(await PayrollProcess.PayRollGet(), true);  
        [HttpPost] public async Task<IActionResult> AddMonthlySalaries() => SendResponse(await PayrollProcess.AddMonthlySalaries(), true); 
        [HttpPost(nameof(updateAppraisal))] public async Task<IActionResult> updateAppraisal([FromBody] UpdateAppraisal data) => SendResponse(await PayrollProcess.updateAppraisal(data), true);

    }
}

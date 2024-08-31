using Alpha.Controllers;
using Hrms.Model;
using Hrms.Process;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Hrms.Provider.AccessProvider;

namespace Hrms.Controllers
{
    [Authorize(Roles = nameof(SystemUserType.Admin)), Route("api/[controller]")]
    public class EmployeeController : BaseController
    {
        readonly EmployeeProcess process;
        public EmployeeController([FromServices] Employee employee) { process = new() { CurrentEmployee = employee }; }
        [HttpGet] public async Task<IActionResult> Get() => SendResponse(await process.Get(), false);
        [HttpPost] public async Task<IActionResult> Post([FromBody] Employee data) => SendResponse(await process.Save(data), true);
        [HttpDelete("{id}")] public async Task<IActionResult> Delete([FromRoute] int id) => SendResponse(await process.Delete(id), true);
    }
}

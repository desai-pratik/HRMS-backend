using Hrms.Enums;
using Hrms.Model;
using Hrms.Process;
using Hrms.Provider;
using Microsoft.AspNetCore.Mvc;

namespace Alpha.Controllers
{
    public class BaseController : ControllerBase
    {
        [NonAction]
        public ActionResult SendResponse(ApiResponse apiResponse, bool showMessage = false)
        {
            if (showMessage) { apiResponse.Message ??= Convert.ToString(Enum.Parse<StatusFlags>(Convert.ToString(apiResponse.Status))).AddSpaceBeforeCapital(); }
            return apiResponse.Status == (byte)StatusFlags.Failed ? BadRequest(apiResponse) : Ok(apiResponse);
        }
    }

    [ApiController, Route("api/[controller]")]
    public class UtilityController : BaseController
    {
        [HttpGet(nameof(MigrateData))] public async Task<IActionResult> MigrateData() => SendResponse(await UtilityProcess.Migrate(), true);
        [HttpGet(nameof(SeedData))] public async Task<IActionResult> SeedData() => SendResponse(await UtilityProcess.Seed(), true);
    }
}

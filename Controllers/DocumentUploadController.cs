using Alpha.Controllers;
using Hrms.Enums;
using Hrms.Model;
using Hrms.Process;
using Hrms.Provider;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IO.Compression;
using System.Security.Claims;
using static Hrms.Provider.AccessProvider;

namespace Hrms.Controllers
{
   
    [Authorize(Roles = nameof(SystemUserType.Admin)), Route("api/[controller]"),DisableRequestSizeLimit]
    public class EmployeeDocumentController : BaseController
    {
        [HttpGet(nameof(GetPage))]
        public async Task<IActionResult> GetPage([FromQuery] int EmployeeId) => SendResponse(await HrmsEmployeeDocumentProcess.Get(EmployeeId));

        [HttpPost]
        public async Task<IActionResult> Post(List<IFormFile> files, int EmployeeId) => SendResponse(await HrmsEmployeeDocumentProcess.Save(files, EmployeeId),true);

        [HttpDelete("{EmployeeId}")]
        public  async Task<IActionResult> Delete(int EmployeeId) => SendResponse(await HrmsEmployeeDocumentProcess.Delete(EmployeeId),true);


        [HttpPost(nameof(Download) + "/{EmployeeId}")]
        public async Task<IActionResult> Download(int EmployeeId)
        {
            try
            {
                using (var defaultContext = new DefaultContext())
                {
                    var employee = await defaultContext.Employees.FindAsync(EmployeeId); if (employee == null) { return NotFound(); }
                    string userFolder = Path.Combine(ConfigProvider.Provider.BaseDirectory, employee.Username); if (!Directory.Exists(userFolder)) { return NotFound(); }
                    string zipFileName = $"{employee.Username}_{DateTime.Now:dd-MM-yyyy_HH-mm-ss}.zip";
                    string zipFilePath = Path.Combine(ConfigProvider.Provider.BaseDirectory, zipFileName);
                    ZipFile.CreateFromDirectory(userFolder, zipFilePath);

                    byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(zipFilePath);

                    System.IO.File.Delete(zipFilePath);

                    return File(fileBytes, "application/zip", zipFileName);
                }
            }
            catch (Exception ex)
            {
                LogsProvider.WriteErrorLog(Convert.ToString(ex), null);
                return StatusCode(500);
            }
        }


    }


}



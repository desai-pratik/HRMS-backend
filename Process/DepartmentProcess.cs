using Hrms.Enums;
using Hrms.Model;
using Hrms.Provider;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Process
{
    public class DepartmentProcess
    {
        public static async Task<ApiResponse> Get()
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                using DefaultContext defaultContext = new();
                apiResponse.Data = await defaultContext.Departments.AsNoTracking().ToListAsync();
            }
            catch (Exception ex) { apiResponse.Status = (byte)StatusFlags.Failed; apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }
        public static async Task<ApiResponse> Save(Department data)
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                using DefaultContext defaultContext = new();

                if (data.DeptId == 0 && !await defaultContext.Departments.AsNoTracking().AnyAsync(d => d.DeptName == data.DeptName))
                {
                    await defaultContext.Departments.AddAsync(data);
                }
                 
                else { apiResponse.Status = (byte)StatusFlags.AlreadyExists; }
                _ = await defaultContext.SaveChangesAsync();
            }
            catch (Exception ex) { apiResponse.Status = (byte)StatusFlags.Failed; apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }
    }
}

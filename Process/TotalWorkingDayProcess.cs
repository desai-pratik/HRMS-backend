using Hrms.Enums;
using Hrms.Model;
using Hrms.Provider;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Hrms.Process
{
    public class TotalWorkingDayProcess
    {
        public static async Task<ApiResponse> AddWorkingDay(TotalWorkingDays data)
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                using DefaultContext defaultContext = new();
                TotalWorkingDays workingDays = new TotalWorkingDays()
                {
                    WorkingDays = data.WorkingDays,
                    Month = data.Month,
                    Year = data.Year,
                };
                await defaultContext.TotalWorkingDay.AddAsync(workingDays);
                await defaultContext.SaveChangesAsync();
                apiResponse.Message = "Working Days Added successful"; apiResponse.Status = (byte)StatusFlags.Success;
            }
            catch (Exception ex) { apiResponse.Status = (byte)StatusFlags.Failed; apiResponse.DetailedError = Convert.ToString(ex); }

            return apiResponse;
        }

        public static async Task<ApiResponse> GetWorkingDay()
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                using DefaultContext defaultContext = new();
                var TotalWorkingDay = await defaultContext.TotalWorkingDay.AsNoTracking().ToListAsync(); 
                apiResponse.Data = TotalWorkingDay;
            }
            catch (Exception ex) { apiResponse.Status = (byte)StatusFlags.Failed; apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }
    }
}

using Alpha.Process;
using Hrms.Enums;
using Hrms.Model;
using Hrms.Provider;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Process
{
    public class LeaveApplicationProcess : GlobalVariables
    {
        public async Task<ApiResponse> LeaveRequest(LeaveApplication data)
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Failed };
            try
            {
                DefaultContext defaultContext = new();
                TimeSpan duration = data.EndDate - data.StartDate;
                int totalDays = (int)duration.TotalDays;

                LeaveApplication data1 = new LeaveApplication()
                {
                    EmpId = CurrentEmployee.Id,
                    Username = CurrentEmployee.Username,
                    ApplyDate = DateTime.Now,
                    Reason = data.Reason,
                    LeaveType = data.LeaveType,
                    StartDate = data.StartDate,
                    EndDate = data.EndDate,
                    TotleDayLeave = totalDays,
                    Status = Convert.ToString(LeaveStatus.Pending)
                };
                await defaultContext.LeaveApplication.AddAsync(data1);
                await defaultContext.SaveChangesAsync();
                apiResponse.Message = "Leave sended successful"; apiResponse.Status = (byte)StatusFlags.Success;

                await defaultContext.SaveChangesAsync();
            }
            catch (Exception ex) { apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }
        public async Task<ApiResponse> UpdateLeaveRequestStatus(LeaveApproval data)
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                if (!CurrentEmployee.IsAdmin) { apiResponse.Status = (byte)StatusFlags.NotPermitted; }
                else
                {
                    DefaultContext defaultContext = new();
                    LeaveApplication LeaveApp = await defaultContext.LeaveApplication.AsNoTracking().FirstOrDefaultAsync(d => d.LeaveId == data.LeaveId && d.Status == Convert.ToString(LeaveStatus.Pending));
                    if (LeaveApp != null)
                    {
                        LeaveApp.Status = Convert.ToString(data.IsApproved ? LeaveStatus.Approved : LeaveStatus.Rejected);
                        _ = defaultContext.LeaveApplication.Update(LeaveApp);
                        _ = await defaultContext.SaveChangesAsync();
                        apiResponse.Status = (byte)StatusFlags.Success;
                        apiResponse.Message = data.IsApproved ? "Approved successfully" : "Canceled successfully";
                    }
                    else { apiResponse.Message = "Transaction is not Pending"; }
                }
            }
            catch (Exception ex) { apiResponse.Status = (byte)StatusFlags.Failed; apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }
        public async Task<ApiResponse> GetUserBy()
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                DefaultContext defaultContext = new();
                if (CurrentEmployee.IsAdmin)
                {
                    apiResponse.Data = await defaultContext.LeaveApplication.AsNoTracking().Include(d => d.Employee).ToListAsync();
                }
                else
                {
                    apiResponse.Data = await defaultContext.LeaveApplication.AsNoTracking().Include(d => d.Employee).Where(d => d.EmpId == CurrentEmployee.Id).ToListAsync();
                }
            }
            catch (Exception ex) { apiResponse.Status = (byte)StatusFlags.Failed; apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }
    }
}

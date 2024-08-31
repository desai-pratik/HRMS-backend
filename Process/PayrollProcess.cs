using Hrms.Enums;
using Hrms.Model;
using Hrms.Provider;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Process
{
    public class PayrollProcess
    {
        public static async Task<ApiResponse> updateAppraisal(UpdateAppraisal data)
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Failed };
            DefaultContext defaultContext = new();
            var updateAppraisal = await defaultContext.Payroll.FirstOrDefaultAsync(e => e.EmpId == data.impId && e.SalaryMonth == DateTime.Now.Month - 1);

            if (updateAppraisal != null)
            {
                updateAppraisal.Appraisal = data.AppraisalAmount;
                updateAppraisal.AppraisalDate = DateTime.Now;
                updateAppraisal.Total += data.AppraisalAmount;
                defaultContext.Payroll.Update(updateAppraisal); 
                apiResponse.Message = "Appraisal sended successfully";
                apiResponse.Status = (byte)StatusFlags.Success;

                var employee = await defaultContext.Employees.FindAsync(updateAppraisal.EmpId);
                if (employee != null)
                {
                    employee.CTCSalary = (updateAppraisal.Appraisal * 12) + employee.CTCSalary;
                    defaultContext.Employees.Update(employee);
                    apiResponse.Message = "Appraisal sent successfully and employee CTCSalary updated.";
                    apiResponse.Status = (byte)StatusFlags.Success;
                }
                else
                {
                    apiResponse.Message = "Employee not found.";
                }
            }
            else
            {
                apiResponse.Message = "Employee not found";
            }
            await defaultContext.SaveChangesAsync();
            return apiResponse;
        }
        public static async Task<ApiResponse> AddMonthlySalaries()
        {
            ApiResponse apiResponse = new ApiResponse { Status = (byte)StatusFlags.Failed };
            try
            {
                using (DefaultContext defaultContext = new DefaultContext())
                {
                    int lastMonth = DateTime.Now.Month - 1; 

                    bool salariesAddedForMonth = defaultContext.Payroll.Any(p => p.SalaryMonth == lastMonth);
                    if (!salariesAddedForMonth)
                    {
                        var employees = defaultContext.Employees.ToList();

                        foreach (var employee in employees)
                        {
                            var storeCTCSalary = employee.CTCSalary;
                            double monthlySalary = storeCTCSalary / 12;

                            var totalWorkingDays = defaultContext.TotalWorkingDay.FirstOrDefault(twd => twd.Month == lastMonth);

                            var totalLeaveDays = defaultContext.LeaveApplication
                                 .Where(s => s.EmpId == employee.Id &&
                                             s.Status == Convert.ToString(LeaveStatus.Approved) &&
                                             s.LeaveType == Convert.ToString(LeaveType.Paid) &&
                                             s.EndDate.Month == lastMonth)
                                 .Sum(s => s.TotleDayLeave);
                            double adjustedSalary = monthlySalary - (totalLeaveDays * (monthlySalary / totalWorkingDays.WorkingDays));
                            Payroll payrollData = new Payroll()
                            {
                                EmpId = employee.Id,
                                Username = employee.Username,
                                SalaryMonth = lastMonth,
                                CurrentSalary = monthlySalary,
                                Total = adjustedSalary,
                                TotalLeave = totalLeaveDays,
                                TotalAttendance = totalWorkingDays.WorkingDays - totalLeaveDays,
                                Appraisal = 0,
                                TotalDays = totalWorkingDays.WorkingDays
                            };

                            defaultContext.Payroll.Add(payrollData);
                        }

                        await defaultContext.SaveChangesAsync();

                        apiResponse.Message = "Salary added successfully for all employees in the last month.";
                        apiResponse.Status = (byte)StatusFlags.Success;
                    }
                    else
                    {
                        apiResponse.DetailedError = "Salary already added for all employees in the last month.";
                    }
                }
            }
            catch (Exception ex)
            {
                apiResponse.Status = (byte)StatusFlags.Failed;
                apiResponse.DetailedError = ex.ToString();
            }
            return apiResponse;
        } 

        public static async Task<ApiResponse> PayRollGet()
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                using (DefaultContext defaultContext = new DefaultContext())
                {
                    apiResponse.Data = await defaultContext.Payroll.AsNoTracking().ToListAsync();
                }
            }
            catch (Exception ex)
            {
                apiResponse.Status = (byte)StatusFlags.Failed;
                apiResponse.DetailedError = ex.ToString();
            }
            return apiResponse;
        }
    }
}

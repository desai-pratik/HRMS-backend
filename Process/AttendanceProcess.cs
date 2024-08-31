using Hrms.Enums;
using Hrms.Model;
using Hrms.Provider;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Globalization;

namespace Hrms.Process
{
    public class AttendanceProcess
    {
        public static async Task<ApiResponse> GetAttendance()
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                using DefaultContext defaultContext = new();
                var result = await defaultContext.Attendances.AsNoTracking().ToListAsync();
                if (result != null)
                {
                    apiResponse.Data = result;
                    apiResponse.Status = (byte)StatusFlags.Success;
                    apiResponse.Message = "Data Retrived";
                    return apiResponse;
                }
                else
                {
                    apiResponse.Message = "No Data Available";
                    return apiResponse;
                }

            }
            catch (Exception ex) { apiResponse.Status = (byte)StatusFlags.Failed; apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }
        public static async Task<ApiResponse> AddAttendance(Attendance data)
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                using DefaultContext defaultContext = new();
                bool existingAttendance = await defaultContext.Attendances
                    .AnyAsync(att => att.EmpId == data.EmpId && att.Date.Date == data.Date.Date);

                if (existingAttendance)
                {
                    apiResponse.Message = "Attendance already added for this user on the specified date.";
                    apiResponse.Status = (byte)StatusFlags.Failed;
                    return apiResponse;
                }
                else
                {
                    TimeSpan workingHours = data.EndTime - data.StartTime;
                    Attendance attendance = new Attendance()
                    {
                        EmpId = data.EmpId,
                        Date = data.Date,
                        StartTime = data.StartTime,
                        EndTime = data.EndTime,
                        TotalWorkingHours = workingHours.TotalHours,
                        Overtime = Math.Max(0, workingHours.TotalHours - 9),
                        OffTime = Math.Max(0, 9 - workingHours.TotalHours),
                    };

                    await defaultContext.Attendances.AddAsync(attendance);
                    await defaultContext.SaveChangesAsync();

                    // Retrieve aggregated data for the current employee
                    var attendanceData = defaultContext.Attendances
                        .Where(att => att.EmpId == data.EmpId)
                        .GroupBy(att => new { att.Date.Year, att.Date.Month })
                        .Select(group => new
                        {
                            Year = group.Key.Year,
                            Month = group.Key.Month,
                            TotalWorkingHours = group.Sum(att => att.TotalWorkingHours),
                            TotalOvertime = group.Sum(att => att.Overtime),
                            TotalOffTime = group.Sum(att => att.OffTime),
                            LastDateOfMonth = group.Max(att => att.Date)
                        })
                        .ToList();

                    string folderPath = $"E:/Divya/Workspace/Attendance_{data.EmpId}";
                    Directory.CreateDirectory(folderPath);

                    foreach (var record in attendanceData)
                    {
                        string filePath = $"{folderPath}/attendance_data_{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(record.Month)}_{record.Year}.xlsx";
                        FileInfo fileInfo = new FileInfo(filePath);
                        ExcelPackage excelPackage;
                        if (fileInfo.Exists)
                        {
                            using (var stream = new FileStream(filePath, FileMode.Open))
                            {
                                excelPackage = new ExcelPackage(stream);
                            }
                        }
                        else
                        {
                            excelPackage = new ExcelPackage();
                        }

                        ExcelWorksheet worksheet;
                        if (excelPackage.Workbook.Worksheets.Any())
                        {
                            worksheet = excelPackage.Workbook.Worksheets.First();
                        }
                        else
                        {
                            worksheet = excelPackage.Workbook.Worksheets.Add("Attendance");
                            // Add headers
                            worksheet.Cells[1, 1].Value = "Date";
                            worksheet.Cells[1, 2].Value = "TotalWorkingHours";
                            worksheet.Cells[1, 3].Value = "TotalOvertime";
                            worksheet.Cells[1, 4].Value = "TotalOffTime";
                        }

                        // Add data for the current month
                        int rowCount = worksheet.Dimension?.Rows ?? 1;
                        worksheet.Cells[rowCount + 1, 1].Value = record.LastDateOfMonth.ToString("yyyy-MM-dd"); // Last date of the month in "yyyy-MM-dd" format
                        worksheet.Cells[rowCount + 1, 2].Value = record.TotalWorkingHours;
                        worksheet.Cells[rowCount + 1, 3].Value = record.TotalOvertime;
                        worksheet.Cells[rowCount + 1, 4].Value = record.TotalOffTime;

                        excelPackage.SaveAs(fileInfo);
                    }

                    apiResponse.Message = $"Attendance Added and exported to Excel for Employee ID {data.EmpId}";
                    apiResponse.Status = (byte)StatusFlags.Success;
                }
            }
            catch (Exception ex)
            {
                apiResponse.Status = (byte)StatusFlags.Failed;
                apiResponse.DetailedError = Convert.ToString(ex);
            }

            return apiResponse;
        }





    }
}

using Alpha.Process;
using Hrms.Enums;
using Hrms.Model;
using Hrms.Provider;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Pqc.Crypto.Lms;
using System.IO.Compression;

namespace Hrms.Process
{
    public class HrmsEmployeeDocumentProcess : GlobalVariables
    {

        public static async Task<ApiResponse> Get(int EmployeeId)
        {
            ApiResponse apiResponse = new ApiResponse { Status = (byte)StatusFlags.Success };
            try
            {
                using (var defaultContext = new DefaultContext())
                {
                    var employee = await defaultContext.Employees.FindAsync(EmployeeId);
                    if (employee == null)
                    {
                        apiResponse.Message = "Employee not found";
                        return apiResponse;
                    }

                    var documents = await defaultContext.HrmsEmployeeDocument
                        .Where(doc => doc.EmployeeId == EmployeeId)
                        .Select(doc => doc.DocumentName)
                        .ToListAsync();

                    if (documents.Count == 0)
                    {
                        apiResponse.Data = employee;
                    }
                    else
                    {
                       
                        apiResponse.Data = new
                        {
                            Employee = employee,
                            Documents = documents
                        };
                    }

                }
            }
            catch (Exception ex)
            {
                LogsProvider.WriteErrorLog(Convert.ToString(ex), null);
                return new ApiResponse { Status = (byte)StatusFlags.Failed, DetailedError = Convert.ToString(ex) };
            }
            return apiResponse;
        }

      
        public static async Task<ApiResponse> Save(List<IFormFile> files, int EmployeeId)
        {
            ApiResponse apiResponse = new ApiResponse { Status = (byte)StatusFlags.Success };
            try
            {
                using (DefaultContext defaultContext = new DefaultContext())
                {
                    var data = await defaultContext.Employees.FindAsync(EmployeeId);

                    if (data != null)
                    {
                        string userFolder = Path.Combine(ConfigProvider.Provider.BaseDirectory, data.Username);

                        foreach (var file in files)
                        {
                            // Check if the file is a PDF
                            if (Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                            {
                                string fileName = await FileProvider.ReadFileToPath(file, userFolder);

                                var document = new HrmsEmployeeDocument
                                {
                                    EmployeeId = data.Id,
                                    DocumentName = file.FileName,
                                    DocumentUrl = Path.Combine(fileName)
                                };

                                defaultContext.HrmsEmployeeDocument.Add(document);
                            }
                            else
                            {
                                // Skip non-PDF files
                                apiResponse.Message = ($"Skipping file {file.FileName} because it's not a PDF.");
                            }
                        }

                        await defaultContext.SaveChangesAsync();

                        apiResponse.Data = files.Where(file => Path.GetExtension(file.FileName).Equals(".pdf", StringComparison.OrdinalIgnoreCase))
                                               .Select(file => file.FileName).ToList();
                    }
                    else
                    {
                        apiResponse.Message = "Emp Not found";
                    }
                }
            }
            catch (Exception ex)
            {
                LogsProvider.WriteErrorLog(Convert.ToString(ex), null);
                apiResponse.Status = (byte)StatusFlags.Failed;
                apiResponse.DetailedError = Convert.ToString(ex);
            }
            return apiResponse;
        }

        public static async Task<ApiResponse> Delete(int EmployeeId)
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                using (var defaultContext = new DefaultContext())
                {
                
                    var employee = await defaultContext.Employees.FirstOrDefaultAsync(e => e.Id == EmployeeId);
                    if (employee != null)
                    {
                        var documents = await defaultContext.HrmsEmployeeDocument.Where(doc => doc.EmployeeId == EmployeeId).ToListAsync();
                        defaultContext.HrmsEmployeeDocument.RemoveRange(documents);
                        await defaultContext.SaveChangesAsync();


                        string userFolder = Path.Combine(ConfigProvider.Provider.BaseDirectory, employee.Username);
                        if (Directory.Exists(userFolder))
                        {
                            Directory.Delete(userFolder, true);
                        }
                    }

                    else
                    {
                        apiResponse.Message = "Emp Not found";
                    }
  
                }
            }
            catch (Exception ex)
            {
                LogsProvider.WriteErrorLog(Convert.ToString(ex), null);
                apiResponse.Status = (byte)StatusFlags.Failed;
                apiResponse.DetailedError = Convert.ToString(ex);
               
            }
            return apiResponse;
        }
    }


}


using Alpha.Process;
using Crypt.Providers;
using Hrms.Enums;
using Hrms.Model;
using Hrms.Provider;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Process
{
    public class EmployeeProcess : GlobalVariables
    {
        public async Task<ApiResponse> Get()
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                using DefaultContext defaultContext = new();
                var employees = await defaultContext.Employees.AsNoTracking().ToListAsync();
                foreach (var employee in employees)
                {
                    employee.Password = EncryptionProvider.Decrypt(employee.Password);
                }
                apiResponse.Data = employees;
            }
            catch (Exception ex) { apiResponse.Status = (byte)StatusFlags.Failed; apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }


        public async Task<ApiResponse> Save(Employee data)
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Failed };
            try
            {
                DefaultContext defaultContext = new();
                var existingEmployee = await defaultContext.Employees.FirstOrDefaultAsync(e => e.Id == data.Id);
                if (existingEmployee == null)
                {
                    if (defaultContext.Employees.Any(d => d.Username == data.Username)) { apiResponse.Message = "User already registered with anothe user"; }
                    else if (defaultContext.Employees.Any(d => d.Phone == data.Phone)) { apiResponse.Message = "Contact number already registered with anothe user"; }
                    else
                    {
                        var lastEmployeeId = await defaultContext.Employees
                            .OrderByDescending(e => e.Id)
                            .Select(e => e.Id)
                            .FirstOrDefaultAsync() + 1;
                        data.Username = data.FirstName + lastEmployeeId;
                        string password = GenerateUserPassword(data.FirstName);
                        data.Password = EncryptionProvider.Encrypt(password);
                        if (data.DepartmentName != null)
                        {
                            Department department = await defaultContext.Departments.AsNoTracking().FirstOrDefaultAsync(d => d.DeptName == data.DepartmentName);
                            if (department != null)
                            {
                                data.DeptId = department.DeptId;
                                data.DepartmentName = data.DepartmentName;
                            }
                        }
                        else
                        {
                            apiResponse.Message = "Select the Department name";
                        }
                        data.IsAdmin = false;
                        data.Department = null;
                        _ = await defaultContext.Employees.AddAsync(data);
                        await defaultContext.SaveChangesAsync();
                        apiResponse.Message = "Registration successful"; apiResponse.Status = (byte)StatusFlags.Success;
                    }
                }
                else
                {
                    var existingEmployee1 = await defaultContext.Employees.FirstOrDefaultAsync(e => e.Id == data.Id);
                    if (existingEmployee1 != null)
                    {
                        // Update employee details
                        existingEmployee1.FirstName = data.FirstName;
                        var lastEmployeeId = await defaultContext.Employees
                            .OrderByDescending(e => e.Id)
                            .Select(e => e.Id)
                            .FirstOrDefaultAsync() + 1;
                        existingEmployee1.Username = data.FirstName + lastEmployeeId;
                        string password = GenerateUserPassword(existingEmployee1.FirstName);
                        existingEmployee1.Password = EncryptionProvider.Encrypt(password);
                        // Update other fields as needed
                        existingEmployee1.IsAdmin = false;
                        existingEmployee1.Email = data.Email;
                        existingEmployee1.LastName = data.LastName;
                        existingEmployee1.KautilyamEmail = data.KautilyamEmail;
                        existingEmployee1.Phone = data.Phone;
                        existingEmployee1.Address = data.Address;
                        existingEmployee1.City = data.City;
                        existingEmployee1.State = data.State;
                        existingEmployee1.PinCode = data.PinCode;
                        existingEmployee1.CTCSalary = data.CTCSalary;
                        existingEmployee1.Country = data.Country;
                        existingEmployee1.NativePlace = data.NativePlace;
                        existingEmployee1.DepartmentName = data.DepartmentName;
                        existingEmployee1.Designation = data.Designation;
                        existingEmployee1.JoiningDate = data.JoiningDate;
                        existingEmployee1.AadharCardNumber = data.AadharCardNumber;
                        existingEmployee1.PanCardNumber = data.PanCardNumber;
                        existingEmployee1.IFSCCode = data.IFSCCode;
                        existingEmployee1.BankAccountNumber = data.BankAccountNumber;
                        existingEmployee1.GpayNumber = data.GpayNumber;

                        defaultContext.Employees.Update(existingEmployee1);
                        await defaultContext.SaveChangesAsync();
                        apiResponse.Message = "Employee details updated successfully";
                        apiResponse.Status = (byte)StatusFlags.Success;
                    }
                    else
                    {
                        apiResponse.Message = "Employee not found";
                    }
                }
            }
            catch (Exception ex) { apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }

        public static string GenerateUserPassword(string FirstName)
        {
            using DefaultContext defaultContext = new();
            string password;
            if (FirstName.Length <= 3)
            {
                do { password = $"{FirstName}@{new Random().Next(10000, 99999):D5}"; } while (defaultContext.Employees.AsNoTracking().Any(e => e.Password == password));
            }
            else
            {
                do { password = $"{FirstName}@{new Random().Next(100, 999):D3}"; } while (defaultContext.Employees.AsNoTracking().Any(e => e.Password == password));
            }
            return password;
        }
        public async Task<ApiResponse> Delete(int id)
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                DefaultContext defaultContext = new();
                Employee data = await defaultContext.Employees.AsNoTracking().FirstOrDefaultAsync(d => d.Id == id && !d.IsAdmin);
                if (data != null && CurrentEmployee.IsAdmin) { defaultContext.Employees.Remove(data); await defaultContext.SaveChangesAsync(); }
                else { apiResponse.Message = "User not found"; }
            }
            catch (Exception ex) { apiResponse.Status = (byte)StatusFlags.Failed; apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }
    }
}

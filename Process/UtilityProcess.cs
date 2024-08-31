using Hrms.Enums;
using Hrms.Model;
using Hrms.Provider;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Process
{
    public class UtilityProcess
    {
        public static async Task<ApiResponse> Migrate()
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                DefaultContext defaultContext = new();
                await defaultContext.Database.MigrateAsync();
            }
            catch (Exception ex) { apiResponse.Status = (byte)StatusFlags.Failed; apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }
        public static async Task<ApiResponse> Seed()
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Success };
            try
            {
                DefaultContext defaultContext = new();
                await defaultContext.Database.MigrateAsync();
                await SeedUser(defaultContext, new Employee { FirstName = "Admin", LastName = "Admin", Username = "Admin", Password = EncryptionProvider.Encrypt("Admin"), Email = "admin@gmail.com", KautilyamEmail ="admin@kautilyam.com",Phone = "8767676567", JoiningDate = DateTime.Now, IsAdmin = true });
                await SeedDepartment(defaultContext, new Department { DeptName = "Human Resource" });
                await SeedDepartment(defaultContext, new Department { DeptName = "Research and Development" });
                _ = await defaultContext.SaveChangesAsync();

            }
            catch (Exception ex) { apiResponse.Status = (byte)StatusFlags.Failed; apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }
        public static async Task SeedUser(DefaultContext context, Employee data)
        {
            if (!await context.Employees.AnyAsync(d => d.Username == data.Username)) { _ = await context.Employees.AddAsync(data); }
        }

        public static async Task SeedDepartment(DefaultContext context, Department data)
        {
            if (!await context.Departments.AnyAsync(d => d.DeptName == data.DeptName)) { _ = await context.Departments.AddAsync(data); }
        }
    }
}

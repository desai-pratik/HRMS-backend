using Crypt.Providers;
using Hrms.Enums;
using Hrms.Model;
using Hrms.Provider;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static Hrms.Provider.AccessProvider;

namespace Hrms.Process
{
    public class LoginProcess
    {
        public static async Task<ApiResponse> Login(AuthModel data)
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Failed };
            try
            {
                DefaultContext defaultContext = new();
                Employee employee = await defaultContext.Employees.AsNoTracking().FirstOrDefaultAsync(u => u.Username == data.Username && u.Password == EncryptionProvider.Encrypt(data.Password));

                if (employee == null) { apiResponse.Message = "Enter valid credentials"; }
                else
                {
                    employee.Password = ""; DateTime expiry = DateTime.Now.Add(TimeSpan.FromHours(24) - DateTime.Now.TimeOfDay);
                    Claim additionalClaim = new(ClaimTypes.Role, employee.IsAdmin ? Convert.ToString(SystemUserType.Admin) : Convert.ToString(SystemUserType.User));
                    employee.AccessToken = GetUserAccessToken(employee, expiry, additionalClaim);
                    apiResponse.Data = employee; apiResponse.Status = (byte)StatusFlags.Success;
                }
            }
            catch (Exception ex) { apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }
        public static async Task<ApiResponse> ForgotPassword(string username)
        {
            ApiResponse apiResponse = new() { Status = (byte)StatusFlags.Failed };
            try
            {
                DefaultContext defaultContext = new();
                Employee employee = await defaultContext.Employees.AsNoTracking().Where(d => d.Username == username).OrderBy(d => d.Id).LastOrDefaultAsync();
                EmailTemplate emailTemplate = new EmailTemplate();
                {
                    emailTemplate.Name = employee.Username;
                    emailTemplate.Type = "ResetPassword";
                    emailTemplate.Template = $"Your Userid is {employee.Username} and your Password is: {EncryptionProvider.Decrypt(employee.Password)}";
                    emailTemplate.Subject = "Password Retrieved";

                }
                defaultContext.Add(emailTemplate);
                await defaultContext.SaveChangesAsync();
                if (employee != null)
                {
                    employee.Password = EncryptionProvider.Decrypt(employee.Password);
                    apiResponse.Status = (byte)StatusFlags.Success;
                    apiResponse.Message = "Password sent to your Email Address Successfully";

                    // Send email
                    EmailTemplate email = await defaultContext.EmailTemplates.AsNoTracking().Where(d => d.Type == EmailTemplateType.ResetPassword.ToString() && d.Name == employee.Username).OrderBy(d => d.Id).LastOrDefaultAsync();
                    if (email != null)
                    {
                        string mailBody = FunctionProvider.FillTemplate(employee, email.Template);
                        MailConfig mail = new() { ReceiverEmail = employee.Email, Subject = email.Subject, MailBody = mailBody };
                        _ = await MailProvider.SendMail(mail);
                    }

                }
                else { apiResponse.Message = "Employee not exist with this user id"; }
            }
            catch (Exception ex) { apiResponse.Status = (byte)StatusFlags.Failed; apiResponse.DetailedError = Convert.ToString(ex); }
            return apiResponse;
        }
    }
}

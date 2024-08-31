using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Hrms.Model
{
    public class Employee
    {
        [Key] public int Id { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string Phone { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string KautilyamEmail { get; set; }
        [Required] public DateTime JoiningDate { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public int PinCode { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
         public bool IsAdmin { get; set; } = false;
        public string NativePlace { get; set; }
        public string Designation { get; set; } 
        [NotMapped] public string AccessToken { get; set; }
        public string DepartmentName { get; set; }
        public int? DeptId { get; set; }
        [ForeignKey(nameof(DeptId))] public Department Department { get; set; }
        public double CTCSalary { get; set; }
        public string AadharCardNumber { get; set; }
        public string PanCardNumber { get; set; }
        public string GpayNumber { get; set; }
        public string BankAccountNumber { get; set; }
        public string IFSCCode { get; set; }
    }

    public class Department
    {
        [Key] public int DeptId { get; set; }
        public string DeptName { get; set; }
    }
    public class EmailTemplate
    {
        [Key] public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Type { get; set; }
        [Required] public string Template { get; set; }
        [Required] public string Subject { get; set; }
    }

    public class Attendance
    {
        [Key] public int Id { get; set; }
        public int EmpId { get; set; }
        [ForeignKey(nameof(EmpId))] public Employee Employee { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public DateTime StartTime { get; set; } = DateTime.Today.AddHours(9).AddMinutes(30);
        public DateTime EndTime { get; set; } = DateTime.Today.AddHours(19);
        public double? TotalWorkingHours { get; set; }
        public double? Overtime { get; set; }
        public double? OffTime { get; set; }

    }
    public class LeaveApplication
    {
        [Key] public int LeaveId { get; set; }
        public int EmpId { get; set; }
        [ForeignKey(nameof(EmpId))] public Employee Employee { get; set; }
        [Required] public string Username { get; set; }
        [Required] public DateTime ApplyDate { get; set; } = DateTime.Now;
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime EndDate { get; set; }
        public int TotleDayLeave { get; set; }
        [Required] public string Reason { get; set; }
        [Required] public string LeaveType { get; set; }
        public string Status { get; set; }
    }

    public class TotalWorkingDays
    {
        [Key] public int Id { get; set; }
        public int WorkingDays { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public abstract class EmployeeSubPart
    {
        [Key] public int Id { get; set; }
        [Required] public int EmployeeId { get; set; }
        [JsonIgnore][ForeignKey(nameof(EmployeeId))] public Employee Employee { get; set; }
    }

    [Table(nameof(HrmsEmployeeDocument))]
    public class HrmsEmployeeDocument : EmployeeSubPart
    {
        [Required] public string DocumentName { get; set; }
        public string Number { get; set; }
        public string DocumentUrl { get; set; }
    }
    public class Payroll
    {
        [Key] public int Id { get; set; }
        public int EmpId { get; set; }
        [ForeignKey(nameof(EmpId))] public Employee Employee { get; set; }
        [Required] public string Username { get; set; }
        [NotMapped]public int WorkingDay { get; set; }
        [NotMapped][ForeignKey(nameof(WorkingDay))] public TotalWorkingDays TotalWorkingDays { get; set; }
        public int TotalDays { get; set; }
        public int TotalLeave { get; set; }
        public int TotalAttendance { get; set; }
        public int SalaryMonth { get; set; }
        public double CurrentSalary { get; set; }
        public DateTime AppraisalDate { get; set; } = DateTime.Now;
        public int Appraisal { get; set; }
        public double Total { get; set; }
    } 
}

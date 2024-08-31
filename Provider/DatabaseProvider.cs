using Hrms.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Hrms.Provider
{
    public class DefaultContext : DbContext
    { 
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { base.OnConfiguring(optionsBuilder.UseSqlServer(GetSqlServerConnection())); }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<HrmsEmployeeDocument> HrmsEmployeeDocument { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LeaveApplication> LeaveApplication { get; set; }
        public DbSet<Payroll> Payroll { get; set; }
        public DbSet<TotalWorkingDays> TotalWorkingDay { get; set; } 
        private static string GetSqlServerConnection()
        {
            SqlConnectionStringBuilder connectionBuilder = new()
            {
                ConnectTimeout = 0,
                DataSource = ".",
                UserID = "sa",
                Password = "Avni@003",
                InitialCatalog = "HRMSMaster",
                TrustServerCertificate = true,
                IntegratedSecurity = false
            };
            return connectionBuilder.ConnectionString;
        }
    }
}
//public DbSet<Package> Package { get; set; }
//public DbSet<SponsorData> SponsorData { get; set; }

//Add-Migration FirstMigration -Context DefaultContext -o Migrations
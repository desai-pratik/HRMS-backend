using Hrms.Model;

namespace Alpha.Process
{
    public class GlobalVariables : IDisposable
    {
        public Employee CurrentEmployee { get; set; } 
        public void Dispose() { GC.SuppressFinalize(this); }
    }
}

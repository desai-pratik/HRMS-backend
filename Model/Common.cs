using System.ComponentModel.DataAnnotations;

namespace Hrms.Model
{
    public struct ApiResponse
    {
        public byte Status { get; set; }
        public string Message { get; set; }
        public string DetailedError { get; set; }
        public object Data { get; set; }
    }

    public class AuthModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public abstract class TransectionKeys
    {
        [Key] public int Id { get; set; }

        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;
    }

    public class LeaveApproval
    {
        public int LeaveId { get; set; }
        public bool IsApproved { get; set; }
    }
    public class UpdateAppraisal
    {
        public int impId { get; set; }
        public int AppraisalAmount { get; set; }
    }
}

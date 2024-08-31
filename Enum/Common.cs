namespace Hrms.Enums
{
    public enum StatusFlags : byte
    {
        Success = 1,
        Failed = 2,
        AlreadyExists = 3,
        DependencyExists = 4,
        NotPermitted = 5
    }
    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected
    }
    public enum LeaveType
    {
        Paid,
        Unpaid,
        Sick,
        Casual,
        Vacation
    }

    public enum EmailTemplateType
    {
        Registration,
        ResetPassword,
    }
}

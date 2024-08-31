using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Hrms.Model
{
    public class User : TransectionKeys
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        [Required] public bool IsUniversal { get; set; } = false;
        [Required] public bool IsActive { get; set; } = true;
        [Required] public bool IsDeleted { get; set; } = false;

        [Required] public int UserTypeId { get; set; }
        [ForeignKey(nameof(UserTypeId))] public UserType UserType { get; set; }

        [NotMapped] public string TenantCode { get; set; }
        [NotMapped] public string AccessToken { get; set; }
        [NotMapped] public string UserTypeName { get; set; }
    }

    public class UserType : TransectionKeys
    {
        [Required] public string Name { get; set; }
        [Required] public bool IsAdmin { get; set; } = false;
        [Required] public bool IsActive { get; set; } = true;
        [Required] public bool IsDeleted { get; set; } = false;
    }
}

using Microsoft.AspNetCore.Identity;

namespace EmployeeIdentity.APIs.Data.Models
{
    public class Employee : IdentityUser
    {
        public string Department { get; set; } = string.Empty;

    }

}

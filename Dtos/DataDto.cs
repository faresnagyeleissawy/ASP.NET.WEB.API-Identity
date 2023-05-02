using Microsoft.AspNetCore.Mvc;

namespace EmployeeIdentity.APIs.Dtos
{
    public class DataDto 
    {
       public string Ms{ get; set; }=string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}

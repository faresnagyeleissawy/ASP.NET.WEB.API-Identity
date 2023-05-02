using EmployeeIdentity.APIs.Data.Models;
using EmployeeIdentity.APIs.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeIdentity.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        public UserManager<Employee> _userManager;

        public DataController(UserManager<Employee> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        [Authorize]
        [Route("getSecuriedData")]
        public async Task <ActionResult<DataDto>> GetSecuiredData()
        {
           var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            Employee? employeeDB = await _userManager.FindByIdAsync(userId);
            if (employeeDB == null) { return BadRequest(); }


            return new DataDto
            {
                Department = employeeDB.Department,
                Email = employeeDB.Email,
                Ms = "all is good"
            };
        }

        [HttpGet]
        [Authorize]
        [Route("getAllData")]
        public ActionResult <DataDto> GetAllData()
        {
            return new DataDto
            {
                Ms = "all is good"
            };
        }

        [HttpGet]
        [Authorize(Policy ="Manager")]
        [Route("getManagerData")]
        public ActionResult<DataDto> GetManagerData()
        {
            return new DataDto
            {
                Ms = "Manager data returned"
            };
        }

        [HttpGet]
        [Authorize(Policy = "Employee")]
        [Route("getEmployeesData")]
        public ActionResult<DataDto> GetEmployeesData()
        {
            return new DataDto
            {
                Ms = "Employees data returned"
            };
        }
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using EmployeeIdentity.APIs.Data.Models;

namespace EmployeeIdentity.APIs.Data.Context
{
    public class DepartmentContext : IdentityDbContext<Employee>
    {
        public DepartmentContext(DbContextOptions<DepartmentContext> options)
            : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Employee>().ToTable("Employees");
            builder.Entity<IdentityUserClaim<string>>().ToTable("EmployeeClaims");

        }

    }
}
